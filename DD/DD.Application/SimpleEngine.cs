﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using Craft.Math;
using Craft.Algorithms;
using Craft.DataStructures.Graph;
using DD.Domain;
using DD.Application.BattleEvents;

namespace DD.Application
{
    public class SimpleEngine : IEngine
    {
        private Dictionary<Creature, int> _creatureIdMap;
        private GraphAdjacencyList<Point2DVertex, EmptyEdge> _wallGraph;
        private Random _random = new Random(0);
        private Scene _scene;
        private HashSet<int> _obstacleIndexes;
        private Queue<Creature> _actingOrder;
        private int[] _previous;
        private int _battleRoundCount;
        private bool _currentCreatureJustMoved;
        private double _moveDistanceRemaningForCurrentCreature;

        public int[] CurrentCreaturePath { get; private set; }

        public bool BattleroundCompleted { get; private set; }

        public bool BattleDecided { get; private set; }

        public List<Creature> Creatures { get; private set; }

        public Creature CurrentCreature { get; private set; }

        public Creature TargetCreature { get; private set; }

        public ObservableObject<int?> SquareIndexForCurrentCreature { get; }

        public ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }

        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }

        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }

        public ObservableObject<bool> BattleHasStarted { get; }

        public ObservableObject<bool> BattleHasEnded { get; }

        public ObservableObject<bool> AutoRunning { get; }

        public bool NextEventOccursAutomatically => CurrentCreature.IsAutomatic ||
                                                    !CurrentPlayerControlledCreatureHasAnyOptionsLeft();

        public ILogger Logger { get; set; }

        public Scene Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;

                if (_scene == null)
                {
                    _obstacleIndexes = new HashSet<int>();
                }
                else
                {
                    Logger?.WriteLine(LogMessageCategory.Information,
                        $"Selected scene: \"{_scene.Name}\"");

                    _obstacleIndexes = new HashSet<int>(_scene.Obstacles
                        .Select(o => o.IndexOfOccupiedSquare(_scene.Columns)));

                    // Establish a graph of the walls for computing visibility
                    var vertices = new List<Point2DVertex>
                    {
                        new Point2DVertex(-0.5, -0.5),
                        new Point2DVertex(_scene.Columns - 0.5, -0.5),
                        new Point2DVertex(_scene.Columns - 0.5, _scene.Rows - 0.5),
                        new Point2DVertex(-0.5, _scene.Rows - 0.5),
                    };

                    _scene.Obstacles
                        .Where(o => o.ObstacleType == ObstacleType.Wall)
                        .Select(o => o.IndexOfOccupiedSquare(_scene.Columns))
                        .Select(i => new Point2D(
                            i.ConvertToXCoordinate(_scene.Columns),
                            i.ConvertToYCoordinate(_scene.Columns)))
                        .ToList()
                        .ForEach(p =>
                        {
                            vertices.Add(new Point2DVertex(p.X - 0.5, p.Y - 0.5));
                            vertices.Add(new Point2DVertex(p.X + 0.5, p.Y - 0.5));
                            vertices.Add(new Point2DVertex(p.X + 0.5, p.Y + 0.5));
                            vertices.Add(new Point2DVertex(p.X - 0.5, p.Y + 0.5));
                        });

                    _wallGraph = new GraphAdjacencyList<Point2DVertex, EmptyEdge>(vertices, false);

                    // Todo: You can eliminate and combine edges
                    Enumerable.Range(0, vertices.Count / 4).ToList().ForEach(i =>
                    {
                        _wallGraph.AddEdge(i * 4, i * 4 + 1);
                        _wallGraph.AddEdge(i * 4 + 1, i * 4 + 2);
                        _wallGraph.AddEdge(i * 4 + 2, i * 4 + 3);
                        _wallGraph.AddEdge(i * 4 + 3, i * 4);
                    });
                }

                InitializeCreatures();
            }
        }

        public BoardTileMode BoardTileMode { get; set; }

        public event EventHandler CreatureKilled;

        public SimpleEngine(
            ILogger logger)
        {
            SquareIndexForCurrentCreature = new ObservableObject<int?>();
            SquareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            SquareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            BattleHasStarted = new ObservableObject<bool>();
            BattleHasEnded = new ObservableObject<bool>();
            AutoRunning = new ObservableObject<bool>();
            Logger = logger;
            _actingOrder = new Queue<Creature>();
        }

        public void Randomize()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public async Task<IBattleEvent> ExecuteNextEvent()
        {
            MoveCreatureResult moveCreatureResult = null;

            // We peek the next attack rather than deque, because we might not use it in this round
            // such as when no enemies are within range and the creature can only move

            Attack attack;

            // Discard unknown attacks
            while (true)
            {
                CurrentCreature.Attacks.TryPeek(out attack);

                if (attack == null ||
                    attack is MeleeAttack)
                {
                    break;
                }

                CurrentCreature.Attacks.Dequeue();
            }

            if (attack != null)
            {
                var potentialTargetsOfMeleeAttack = IdentifyPotentialTargetsOfMeleeAttack().ToList();

                if (potentialTargetsOfMeleeAttack.Any())
                {
                    attack = CurrentCreature.Attacks.Dequeue();
                    var targetCreature = potentialTargetsOfMeleeAttack.First();

                    AttackOpponent(
                        CurrentCreature,
                        targetCreature,
                        attack,
                        false,
                        out var opponentWasHit,
                        out var opponentWasKilled);

                    if (opponentWasKilled)
                    {
                        OnCreatureKilled();

                        if (!OpponentsStillRemaining(CurrentCreature))
                        {
                            BattleHasEnded.Object = true;
                            BattleDecided = true;
                        }
                    }

                    TargetCreature = targetCreature;
                    _currentCreatureJustMoved = false;

                    return new CreatureAttack();
                }
            }
            else
            {
                _moveDistanceRemaningForCurrentCreature = 0.0;
            }

            // Hvis vi når hertil, så kan eller vil væsenet ikke angribe.
            // Så er spørgsmålet, om den vil bevæge sig, eller alternativt overlade turen til næste væsen

            if (!_currentCreatureJustMoved && _moveDistanceRemaningForCurrentCreature > 0)
            {
                if (moveCreatureResult == null)
                {
                    moveCreatureResult = await DetermineDestinationOfCurrentCreatureWithMeleeAttack();
                }

                if (moveCreatureResult.IndexOfDestinationSquare.HasValue)
                {
                    return Move(moveCreatureResult);
                }
            }

            return new CreaturePass();
        }

        public IBattleEvent PlayerSelectSquare(
            int squareIndex)
        {
            if (!BattleHasStarted.Object ||
                BattleHasEnded.Object ||
                AutoRunning.Object)
            {
                return null;
            }

            if (SquareIndexesCurrentCreatureCanMoveTo.Object != null &&
                SquareIndexesCurrentCreatureCanMoveTo.Object.Keys.Contains(squareIndex))
            {
                // Player decides to move current creature
                _moveDistanceRemaningForCurrentCreature -= SquareIndexesCurrentCreatureCanMoveTo.Object[squareIndex];
                SquareIndexesCurrentCreatureCanMoveTo.Object = null;

                // 1) Find ud af, hvilke væsener der er ved siden af væsenet, for hvert af de felter, der indgår i væsenets path
                // 2) Find ud af, hvilke step, der involverer, at current creature bevæger sig væk fra en modstander
                // 3) Hvis det allerede er ved første step, så bevæger væsenet sig ikke, og så er udfaldet, at
                //    de væsener, der forlades, får et opportunity attack.
                //    Hvis det først er ved et senere step at man forlader en modstander, så bevæger væsenet sig,
                //    Måske kan man gøre det, at enginen, gemmer en sekvens af moves og opportunity attacks,
                //    f.eks. move/oa/oa/move/oa/move
                //    dvs udfaldet af dette trin sådan set ingenting, men når så man skal til næste trin, så
                //    skal enginen se, om der ligger noget på "kø"...

                var path = _previous.DeterminePath(squareIndex);

                // Player controlled creature moves ordinarily, without provoking an opportunity attack
                MoveCurrentCreature(squareIndex);
                CurrentCreaturePath = path;
                IdentifyOptionsForCurrentPlayerControlledCreature(true);

                return new CreatureMove();
            }

            if (SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object.Contains(squareIndex))
            {
                // Player decides to perform a melee attack with current creature
                var attack = CurrentCreature.Attacks.Dequeue();
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;

                var opponent = Creatures.Single(c => c.IndexOfOccupiedSquare(Scene.Columns) == squareIndex);

                AttackOpponent(
                    CurrentCreature,
                    opponent,
                    attack,
                    false,
                    out var opponentWasHit,
                    out var opponentWasKilled);

                if (opponentWasKilled && !OpponentsStillRemaining(CurrentCreature))
                {
                    SquareIndexesCurrentCreatureCanMoveTo.Object = null;
                    BattleHasEnded.Object = true;
                    BattleDecided = true;
                }
                else
                {
                    IdentifyOptionsForCurrentPlayerControlledCreature(false);
                }

                TargetCreature = opponent;

                return new CreatureAttack();
            }

            return null;
        }

        public void StartBattle()
        {
            if (_scene == null)
            {
                throw new InvalidOperationException("Assign a scene to the engine before starting a battle");
            }

            Logger?.WriteLine(LogMessageCategory.Information, "  Starting Battle..");

            BattleDecided = false;
            BattleHasStarted.Object = true;
            BattleHasEnded.Object = false;
            _battleRoundCount = 0;

            var message = "    Determining initial acting order of creatures..";
            Logger?.WriteLine(LogMessageCategory.Information, message);

            var creatureTypeToDieRollMap = new Dictionary<CreatureType, int>();
            var dieRollToCreatureMap = new Dictionary<int, Dictionary<CreatureType, List<Creature>>>();

            // Determine the distance from each friendly creature to the closest enemy
            var indexesOfHostileCreatures = Creatures
                .Where(c => c.IsHostile)
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                .ToArray();

            // Determine the distance from each hostile creature to the closest enemy
            var indexesOfFriendlyCreatures = Creatures
                .Where(c => !c.IsHostile)
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                .ToArray();

            var graph = GenerateGraph(_scene.Rows, _scene.Columns);

            graph.ComputeDistances(
                indexesOfHostileCreatures,
                _obstacleIndexes,
                double.MaxValue,
                out var distancesToHostiles,
                out _previous);

            graph.ComputeDistances(
                indexesOfFriendlyCreatures,
                _obstacleIndexes,
                double.MaxValue,
                out var distancesToFriendlies,
                out _previous);

            Creatures.ForEach(c =>
            {
                int dieRoll;
                if (creatureTypeToDieRollMap.ContainsKey(c.CreatureType))
                {
                    dieRoll = creatureTypeToDieRollMap[c.CreatureType];
                }
                else
                {
                    dieRoll = 1 + _random.Next(20) + c.CreatureType.InitiativeModifier;
                    creatureTypeToDieRollMap[c.CreatureType] = dieRoll;
                }

                if (!dieRollToCreatureMap.ContainsKey(dieRoll))
                {
                    dieRollToCreatureMap[dieRoll] = new Dictionary<CreatureType, List<Creature>>();
                }

                if (!dieRollToCreatureMap[dieRoll].ContainsKey(c.CreatureType))
                {
                    dieRollToCreatureMap[dieRoll][c.CreatureType] = new List<Creature>();
                }

                dieRollToCreatureMap[dieRoll][c.CreatureType].Add(c);
            });

            Logger?.WriteLine(LogMessageCategory.Information, "    Initial acting order:");

            var queueNumberInBattleRound = 0;
            foreach (var kvp1 in dieRollToCreatureMap.OrderByDescending(kvp => kvp.Key))
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    var creaturesOfCurrentType = kvp2.Value;

                    if (creaturesOfCurrentType.Count == 1)
                    {
                        var creature = creaturesOfCurrentType.Single();

                        creature.BattleRoundQueueNumber = queueNumberInBattleRound++;

                        Logger?.WriteLine(
                            LogMessageCategory.Information,
                            $"      {queueNumberInBattleRound}: {Tag(creature)}");
                    }
                    else
                    {
                        var distances = creaturesOfCurrentType.First().IsHostile
                            ? distancesToFriendlies
                            : distancesToHostiles;

                        creaturesOfCurrentType
                            .Select(c => new { Creature = c, Distance = distances[c.IndexOfOccupiedSquare(_scene.Columns)] })
                            .OrderBy(cd => cd.Distance)
                            .Select(cd => cd.Creature)
                            .ToList()
                            .ForEach(c =>
                            {
                                c.BattleRoundQueueNumber = queueNumberInBattleRound++;

                                Logger?.WriteLine(
                                    LogMessageCategory.Information,
                                    $"      {queueNumberInBattleRound}: {Tag(c)}");
                            });
                    }
                }
            }

            StartBattleRound();
        }

        public bool CurrentPlayerControlledCreatureHasAnyOptionsLeft()
        {
            return
                SquareIndexesCurrentCreatureCanMoveTo.Object != null &&
                SquareIndexesCurrentCreatureCanMoveTo.Object.Count > 0 ||
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object.Count > 0;
        }

        public bool CanStartBattle()
        {
            return
                Scene != null &&
                !BattleHasStarted.Object &&
                !BattleHasEnded.Object;
        }

        public void InitializeCreatures()
        {
            BattleHasStarted.Object = false;
            BattleHasEnded.Object = false;

            _actingOrder.Clear();
            Creatures = _scene?.Creatures.Select(c => c.Clone()).ToList();

            InitializeCreatureIdMap();

            CurrentCreature = null;

            SquareIndexForCurrentCreature.Object = null;
            SquareIndexesCurrentCreatureCanMoveTo.Object = null;
            SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
            SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;
        }

        public void StartBattleRound()
        {
            Logger?.WriteLine(LogMessageCategory.Information, $"    Starting battle round {++_battleRoundCount}");

            // Todo: Perform updates for each the round (such as trolls regenerating, poison draining life, etc)

            BattleroundCompleted = false;

            EstablishCreatureActingOrder();
            SwitchToNextCreature();
        }

        public void SwitchToNextCreature()
        {
            // Discard creatures that have been killed
            while (_actingOrder.Count > 0 && _actingOrder.Peek().HitPoints <= 0)
            {
                _actingOrder.Dequeue();
            }

            if (_actingOrder.Count == 0)
            {
                CurrentCreature = null;
                BattleroundCompleted = true;
                return;
            }

            CurrentCreature = _actingOrder.Dequeue();
            SquareIndexForCurrentCreature.Object = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

            var message = $"      Turn goes to {Tag(CurrentCreature)}";
            Logger?.WriteLine(LogMessageCategory.Information, message);

            _moveDistanceRemaningForCurrentCreature = CurrentCreature.CreatureType.Movement;

            CurrentCreature.Attacks = new Queue<Attack>(CurrentCreature.CreatureType.Attacks);

            if (!CurrentCreature.IsAutomatic)
            {
                IdentifyOptionsForCurrentPlayerControlledCreature(false);
            }

            _currentCreatureJustMoved = false;
        }

        public string Tag(Creature creature)
        {
            return $"{creature.CreatureType.Name}{_creatureIdMap[creature]}";
        }

        private void EstablishCreatureActingOrder()
        {
            Creatures
                .OrderBy(c => c.BattleRoundQueueNumber)
                .ToList()
                .ForEach(c => _actingOrder.Enqueue(c));
        }

        private async Task<MoveCreatureResult> DetermineDestinationOfCurrentCreatureWithMeleeAttack()
        {
            // If the creature is next to an opponent, it doesn't move
            // Otherwise, it identifies the set of squares that fullfill the following conditions:
            // * The creature can reach them
            // * The walking distance from the square to the closest opponent is as small as possible
            // The optimal destination is the square from this set that has the shortest walking distance
            // from the current position of the creature. If multiple squares in the set share the same
            // shortest distance, it just chooses one of them

            return await Task.Run(() =>
            {
                var currentSquareIndex = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

                var opponents = Creatures
                    .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                    .ToList();

                var closestOpponent = ClosestOpponent(
                    currentSquareIndex,
                    opponents,
                    out var distanceToClosestOpponent);

                if (distanceToClosestOpponent < 1.5)
                {
                    return new MoveCreatureResult
                    {
                        IndexOfDestinationSquare = null,
                        WalkingDistanceToDestinationSquare = null,
                        FinalClosestOpponent = closestOpponent,
                        FinalDistanceToClosestOpponent = distanceToClosestOpponent
                    };
                }

                var opponentIndexes = opponents
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var allyIndexes = Creatures
                    .Where(c => c.IsHostile == CurrentCreature.IsHostile && !c.Equals(CurrentCreature))
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var forbiddenIndexes = new HashSet<int>(_obstacleIndexes.Concat(allyIndexes).Concat(opponentIndexes));

                var graph = GenerateGraph(_scene.Rows, _scene.Columns);

                // Determine where the current creature can go
                graph.ComputeDistances(
                    new[] { currentSquareIndex },
                    forbiddenIndexes,
                    _moveDistanceRemaningForCurrentCreature,
                    out var walkingDistancesForCurrentCreature,
                    out _previous);

                // Determine the walking distances to the opponents
                graph.ComputeDistances(
                    opponentIndexes,
                    _obstacleIndexes,
                    double.MaxValue,
                    out var walkingDistancesToClosestOpponent);

                // Identify the reachable square(s) that is closest to an opponent
                // (Multiple squares may be equally far from the closest opponent)
                var indexesOfOptimalReachablePositions =
                    walkingDistancesForCurrentCreature.IdentifyIndexesLowerThan(999999)
                        .IdentifyIndexesOfMinimumValue(walkingDistancesToClosestOpponent)
                        .ToList();

                // Select the square that has the shortest walking distance from the square currently
                // occupied by the current creature
                var indexOfDestinationSquare = indexesOfOptimalReachablePositions
                    .IdentifyIndexesOfMinimumValue(walkingDistancesForCurrentCreature)
                    .First();

                // Also determine the walking distance, since we need to subtract it from the
                // remaining walking distance of the current creature for the current turn
                var walkingDistance = walkingDistancesForCurrentCreature[indexOfDestinationSquare];

                closestOpponent = ClosestOpponent(
                    indexOfDestinationSquare,
                    opponents,
                    out var finalDistanceToClosestOpponent);

                var result = new MoveCreatureResult
                {
                    IndexOfDestinationSquare = indexOfDestinationSquare,
                    WalkingDistanceToDestinationSquare = walkingDistance,
                    FinalClosestOpponent = closestOpponent,
                    FinalDistanceToClosestOpponent = finalDistanceToClosestOpponent
                };

                // If the creature is unable to move to a better position (such as when it is boxed in)
                // then IndexOfDestinationSquare should be null

                if (indexOfDestinationSquare == currentSquareIndex)
                {
                    var message = $"        {Tag(CurrentCreature)} passes";
                    Logger?.WriteLine(LogMessageCategory.Information, message);

                    result.IndexOfDestinationSquare = null;
                }

                return result;
            });
        }

        private void InitializeCreatureIdMap()
        {
            _creatureIdMap = new Dictionary<Creature, int>();

            var creatureTypeCountMap = new Dictionary<CreatureType, int>();

            Creatures?.ForEach(c =>
            {
                if (!creatureTypeCountMap.ContainsKey(c.CreatureType))
                {
                    creatureTypeCountMap[c.CreatureType] = 0;
                }

                _creatureIdMap[c] = ++creatureTypeCountMap[c.CreatureType];
            });
        }

        private void IdentifyOptionsForCurrentPlayerControlledCreature(
            bool creatureJustMoved)
        {
            // Only allow the current creature to move, if it has any attacks left
            // and if its last action was not a move
            if (CurrentCreature.Attacks.Count > 0 && !creatureJustMoved)
            {
                SquareIndexesCurrentCreatureCanMoveTo.Object = new Dictionary<int, double>(
                    IdentifyIndexesOfSquaresThatTheCurrentCreatureCanMoveTo());
            }
            else
            {
                SquareIndexesCurrentCreatureCanMoveTo.Object = new Dictionary<int, double>();
            }

            if (CurrentCreature.Attacks.Count(a => a is MeleeAttack) > 0)
            {
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = new HashSet<int>(
                    IdentifyPotentialTargetsOfMeleeAttack().Select(c => c.IndexOfOccupiedSquare(_scene.Columns)));
            }
            else
            {
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = new HashSet<int>();
            }
        }

        private Dictionary<int, double> IdentifyIndexesOfSquaresThatTheCurrentCreatureCanMoveTo()
        {
            var indexOfCurrentCreature = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

            var otherCreatureIndexes = Creatures
                .Where(c => !c.Equals(CurrentCreature))
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns));

            var graph = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

            var forbiddenIndexes = new HashSet<int>(_obstacleIndexes.Concat(otherCreatureIndexes));

            graph.ComputeDistances(
                new[] { indexOfCurrentCreature },
                forbiddenIndexes,
                _moveDistanceRemaningForCurrentCreature,
                out var distances,
                out _previous);

            var result = distances
                .Select((d, i) => new { Index = i, Distance = d })
                .Where(x => x.Distance < 999999)
                .ToDictionary(x => x.Index, x => x.Distance);

            result.Remove(indexOfCurrentCreature);

            return result;
        }

        private IEnumerable<Creature> IdentifyPotentialTargetsOfMeleeAttack()
        {
            if (CurrentCreature.Attacks.Count(a => a is MeleeAttack) == 0)
            {
                return new List<Creature>();
            }

            return IdentifyAdjacentOpponents(
                CurrentCreature.IndexOfOccupiedSquare(_scene.Columns));
        }

        private IEnumerable<Creature> IdentifyAdjacentOpponents(
            int tileIndex)
        {
            var positionX = tileIndex.ConvertToXCoordinate(_scene.Columns);
            var positionY = tileIndex.ConvertToYCoordinate(_scene.Columns);

            var currentCreaturePosition = Helpers.GetTileCenterCoordinates(
                positionX, positionY, BoardTileMode);

            return Creatures
                .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                .Select(c => new 
                    { 
                        Creature = c, 
                        Distance = currentCreaturePosition.SquaredDistanceTo(
                            Helpers.GetTileCenterCoordinates(c.PositionX, c.PositionY, BoardTileMode))
                    })
                .Where(cd => cd.Distance < 2.1)
                .Select(cd => cd.Creature);
        }

        private void MoveCurrentCreature(
            int indexOfTargetSquare)
        {
            CurrentCreature.PositionX = indexOfTargetSquare.ConvertToXCoordinate(_scene.Columns);
            CurrentCreature.PositionY = indexOfTargetSquare.ConvertToYCoordinate(_scene.Columns);

            var message =
                $"        {CurrentCreature.CreatureType.Name}{Tag(CurrentCreature)}" +
                $" moves to position (X, Y) = ({CurrentCreature.PositionX}, {CurrentCreature.PositionY})";

            Logger?.WriteLine(LogMessageCategory.Information, message);

            SquareIndexForCurrentCreature.Object = indexOfTargetSquare;
        }

        private Creature ClosestOpponent(
            int squareIndex,
            List<Creature> allOpponents,
            out double distance)
        {
            Creature closestOpponent = null;
            var shortestSqrDistToAnOpponent = double.MaxValue;

            var x = squareIndex.ConvertToXCoordinate(_scene.Columns);
            var y = squareIndex.ConvertToYCoordinate(_scene.Columns);

            var currentCreaturePosition = Helpers.GetTileCenterCoordinates(
                x, y, BoardTileMode);

            allOpponents.ForEach(opponent =>
            {
                var opponentPosition = Helpers.GetTileCenterCoordinates(
                    opponent.PositionX, opponent.PositionY, BoardTileMode);

                var sqrDist = opponentPosition.SquaredDistanceTo(currentCreaturePosition);

                if (sqrDist >= shortestSqrDistToAnOpponent)
                {
                    return;
                }

                closestOpponent = opponent;
                shortestSqrDistToAnOpponent = sqrDist;
            });

            distance = Math.Sqrt(shortestSqrDistToAnOpponent);
            return closestOpponent;
        }

        private void AttackOpponent(
            Creature attacker,
            Creature opponent,
            Attack attack,
            bool disadvantage,
            out bool opponentWasHit,
            out bool opponentWasKilled)
        {
            opponentWasHit = false;
            opponentWasKilled = false;
            var dieRoll = 1 + _random.Next(20);

            if (disadvantage)
            {
                dieRoll = System.Math.Min(dieRoll, 1 + _random.Next(20));
            }

            var message =
                "        " +
                $"{Tag(attacker)} attacks " +
                $"{Tag(opponent)} " +
                $"with a {attack.Name}";

            if (disadvantage)
            {
                message += ", having disadvantage";
            }

            if (attacker.CreatureType.Thaco - dieRoll <= opponent.CreatureType.ArmorClass)
            {
                var damage = 1 + _random.Next(attack.MaxDamage);
                opponent.HitPoints -= damage;

                opponentWasHit = true;

                message += $", causing {damage} in damage";

                if (opponent.HitPoints <= 0)
                {
                    Creatures.Remove(opponent);
                    opponentWasKilled = true;

                    message += $" => {Tag(opponent)} was killed";
                }
            }
            else
            {
                message += $" and misses";
            }

            Logger?.WriteLine(LogMessageCategory.Information, message);
        }

        private bool OpponentsStillRemaining(
            Creature creature)
        {
            return Creatures.Any(c => c.IsHostile != creature.IsHostile);
        }

        private IBattleEvent Move(
            MoveCreatureResult moveCreatureResult)
        {
            _currentCreatureJustMoved = true;

            if (moveCreatureResult.WalkingDistanceToDestinationSquare.HasValue)
            {
                _moveDistanceRemaningForCurrentCreature -=
                    moveCreatureResult.WalkingDistanceToDestinationSquare.Value;
            }

            var path = _previous.DeterminePath(moveCreatureResult.IndexOfDestinationSquare.Value);

            MoveCurrentCreature(moveCreatureResult.IndexOfDestinationSquare.Value);

            CurrentCreaturePath = path;

            return new CreatureMove();
        }

        private IGraph GenerateGraph(
            int rows,
            int columns)
        {
            switch (BoardTileMode)
            {
                case BoardTileMode.Square:
                {
                    return new GraphMatrix8Connectivity(rows, columns);
                }
                case BoardTileMode.Hexagonal:
                {
                    return new GraphHexMesh(rows, columns);
                }
                default:
                {
                    throw new InvalidEnumArgumentException("Unknown board tile mode");
                }
            }
        }

        private void OnCreatureKilled()
        {
            var handler = CreatureKilled;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
