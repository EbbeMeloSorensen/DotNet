using System;
using System.Collections.Generic;
using DD.Domain;

namespace DD.Application
{
    public static class SceneGenerator
    {
        private static CreatureType _knight = new CreatureType(
            "Knight", 20, 3, 12, 0, 4, new List<Attack>
            {
                new MeleeAttack("Longsword", 10),
                new MeleeAttack("Longsword", 10)
            });

        private static CreatureType _archer = new CreatureType(
            "Archer", 10, 6, 13, 10, 6, new List<Attack>
            {
                new RangedAttack("Bow & Arrow", 4, 6),
                new RangedAttack("Bow & Arrow", 4, 6),
                new RangedAttack("Bow & Arrow", 4, 6)
            });

        private static CreatureType _wizard = new CreatureType(
            "Wizard", 8, 8, 6, 0, 6, new List<Attack>
            {
                new RangedAttack("Magic Missile", 6, 10),
                new RangedAttack("Magic Missile", 6, 10),
                new RangedAttack("Magic Missile", 6, 10)
            });

        private static CreatureType _goblinArcher = new CreatureType(
            "Goblin Archer", 20, 7, 13, 0, 6, new List<Attack>
            {
                new RangedAttack("Bow & Arrow", 4, 4),
                new RangedAttack("Bow & Arrow", 4, 4),
                new RangedAttack("Bow & Arrow", 4, 4)
            });

        private static CreatureType _goblin = new CreatureType(
            "Goblin", 8, 5, 20, 0, 6, new List<Attack>
            {
                new MeleeAttack("Short sword", 6)
            });

        private static CreatureType _ironGolem = new CreatureType(
            "Iron Golem", 80, 3, 3, 0, 6, new List<Attack>
            {
                new MeleeAttack("Fist", 10)
            });

        private static CreatureType _boar = new CreatureType(
            "Boar", 10, 7, 17, 0, 15, new List<Attack>
            {
                new MeleeAttack("Fang", 6)
            });

        private static CreatureType _skeleton = new CreatureType(
            "Skeleton", 6, 17, 19, 0, 3, new List<Attack>
            {
                new MeleeAttack("Axe", 6)
            });

        private static CreatureType _blackPanther = new CreatureType(
            "BlackPanther", 12, 4, 17, 0, 5, new List<Attack>
            {
                new MeleeAttack("Claw", 6)
            });

        private static CreatureType _behir = new CreatureType(
            "Behir", 67, 4, 9, 0, 10, new List<Attack>
            {
                new MeleeAttack("Bite", 10),
                new MeleeAttack("Claw", 10),
                new MeleeAttack("Claw", 10),
                new MeleeAttack("Claw", 10)
            });

        private static CreatureType _orange = new CreatureType(
            "Orange", 30, 5, 13, 0, 4, new List<Attack>
            {
                new MeleeAttack("Butt", 10)
            });

        private static CreatureType _redDragon = new CreatureType(
            "Red Dragon", 70, 5, 11, 0, 7, new List<Attack>
            {
                new MeleeAttack("Claw", 8),
                new MeleeAttack("Claw", 8),
                new MeleeAttack("Bite", 10)
            });

        private static CreatureType _panda = new CreatureType(
            "Panda", 100, 2, 7, 20, 20, new List<Attack>
            {
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70),
                new MeleeAttack("Punch", 70)
            });

        public static Scene GenerateScene(int sceneId)
        {
            switch (sceneId)
            {
                case 1:
                    return GenerateScene1();
                case 2:
                    return GenerateScene2();
                case 3:
                    return GenerateScene3();
                case 4:
                    return GenerateScene4();
                case 5:
                    return GenerateScene5();
                case 6:
                    return GenerateScene6();
                case 7:
                    return GenerateScene7();
                case 8:
                    return GenerateScene8();
                case 9:
                    return GenerateScene9();
                case 10:
                    return GenerateScene10();
                case 11:
                    return GenerateScene11();
                case 12:
                    return GenerateScene12();
                case 13:
                    return GenerateScene13();
                case 14:
                    return GenerateScene14();
                case 15:
                    return GenerateScene15();
                case 16:
                    return GenerateScene16();
                case 17:
                    return GenerateScene17();
                case 18:
                    return GenerateScene18();
                case 19:
                    return GenerateScene19();
                case 20:
                    return GenerateScene20();
                case 21:
                    return GenerateScene21();
                case 22:
                    return GenerateScene22();
                case 23:
                    return GenerateScene23();
                default:
                    throw new ArgumentException("Invalid scene id");
            }
        }

        private static Scene GenerateScene1()
        {
            var scene = new Scene("3 Iron Golems vs 30 Boars", 20, 10);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 5));

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 10));

            scene.AddCreature(new Creature(_ironGolem, false), 3, 12);
            scene.AddCreature(new Creature(_ironGolem, false), 4, 12);
            scene.AddCreature(new Creature(_ironGolem, false), 5, 12);

            for (var r = 0; r < 3; r++)
            {
                for (var c = 0; c < 10; c++)
                {
                    scene.AddCreature(new Creature(_boar, true), c, r);
                }
            }

            return scene;
        }

        private static Scene GenerateScene2()
        {
            var scene = new Scene("7 Knights vs 50 Goblins", 16, 16);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 8));

            for (var r = 0; r < 5; r++)
            {
                for (var c = 0; c < 10; c++)
                {
                    if (r == 0 && c < 7)
                    {
                        scene.AddCreature(new Creature(_knight, false) {IsAutomatic = true}, c, r + 12);
                    }

                    scene.AddCreature(new Creature(_goblin, true), c, r);
                }
            }

            return scene;
        }

        private static Scene GenerateScene3()
        {
            var scene = new Scene("8 skeletons vs 2 knights and 2 black panthers", 13, 8);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 5));

            for (var r = 0; r < 1; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    scene.AddCreature(new Creature(_skeleton, true), c, r);
                }
            }

            scene.AddCreature(new Creature(_knight, false), 4, 12);
            scene.AddCreature(new Creature(_knight, false), 5, 12);
            scene.AddCreature(new Creature(_blackPanther, false), 3, 12);
            scene.AddCreature(new Creature(_blackPanther, false), 6, 12);

            return scene;
        }

        private static Scene GenerateScene4()
        {
            var scene = new Scene("1 skeleton vs 1 knight", 8, 8);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddCreature(new Creature(_skeleton, true), 7, 7);
            scene.AddCreature(new Creature(_knight, false), 0, 0);

            return scene;
        }

        private static Scene GenerateScene5()
        {
            var scene = new Scene("2 knights in a house", 28, 23);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 14, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 11));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 12, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 14, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 14));
            scene.AddCreature(new Creature(_knight, false), 14, 10);
            scene.AddCreature(new Creature(_knight, false), 13, 12);

            return scene;
        }

        private static Scene GenerateScene6()
        {
            var scene = new Scene("6 skeletons vs 3 knights & 2 archers", 8, 8);
            scene.AddCreature(new Creature(_skeleton, true), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 7, 0);
            scene.AddCreature(new Creature(_knight, false) {HitPoints = 15}, 3, 3);
            scene.AddCreature(new Creature(_knight, false), 4, 3);
            scene.AddCreature(new Creature(_skeleton, true), 0, 7);
            scene.AddCreature(new Creature(_skeleton, true), 7, 7);
            scene.AddCreature(new Creature(_knight, false), 6, 3);
            scene.AddCreature(new Creature(_skeleton, true), 0, 4);
            scene.AddCreature(new Creature(_skeleton, true), 7, 4);
            scene.AddCreature(new Creature(_archer, false), 4, 4);
            scene.AddCreature(new Creature(_archer, false), 5, 4);

            return scene;
        }

        private static Scene GenerateScene7()
        {
            var scene = new Scene("Antons scene (7)", 22, 28);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 12, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 13, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 14, 7));

            scene.AddCreature(new Creature(_wizard, false), 6, 1);

            return scene;
        }

        private static Scene GenerateScene8()
        {
            var scene = new Scene("16 skeletons vs 16 goblins", 8, 8);

            for (var r = 0; r < 2; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    scene.AddCreature(new Creature(_goblin, false), c, r + 6);

                    scene.AddCreature(new Creature(_skeleton, true), c, r);
                }
            }

            return scene;
        }

        private static Scene GenerateScene9()
        {
            var scene = new Scene("Cecilies bane", 32, 38);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 9));

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 14, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 16, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 17, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 11));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 15));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 16));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 18));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 19));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 21));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 22));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 23));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 24));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 25));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 13, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 11));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 15));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 16));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 18));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 19));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 21));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 22));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 23));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 24));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 25));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 18, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 14, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 15, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 16, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 17, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 20, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 21, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 23, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 24, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 12));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 15));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 16));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 13));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 15));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 16));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 17));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 18));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 19));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 21));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 22));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 23));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 24));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 25));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 26));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 27));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 28));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 25, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 24, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 23, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 21, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 20, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 29));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 24, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 23, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 21, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 20, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 20));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 19));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 19, 18));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 23, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 14));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 15));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 16));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 17));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 22, 18));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 23, 18));

            scene.AddCreature(new Creature(_redDragon, true), 9, 13);
            scene.AddCreature(new Creature(_redDragon, true), 7, 14);
            scene.AddCreature(new Creature(_redDragon, true), 11, 14);
            scene.AddCreature(new Creature(_orange, true), 23, 15);
            scene.AddCreature(new Creature(_orange, true), 23, 16);
            scene.AddCreature(new Creature(_orange, true), 23, 17);

            for (var r = 10; r < 17; r++)
            {
                for (var c = 14; c < 18; c++)
                {
                    scene.AddObstacle(new Obstacle(ObstacleType.Water, c, r));
                }
            }

            scene.AddCreature(new Creature(_panda, false), 24, 24);
            //scene.AddCreature(new Creature(_knight, false), 24, 25);
            //scene.AddCreature(new Creature(_knight, false), 23, 25);
            //scene.AddCreature(new Creature(_behir, false), 23, 26);
            //scene.AddCreature(new Creature(_behir, false), 24, 26);
            //scene.AddCreature(new Creature(_orange, false), 22, 25);
            //scene.AddCreature(new Creature(_orange, false), 22, 26);

            for (var r = 17; r < 21; r++)
            {
                for (var c = 7; c <= 11; c++)
                {
                    scene.AddCreature(new Creature(_blackPanther, true), c, r);
                }
            }

            for (var r = 18; r < 26; r++)
            {
                for (var c = 14; c < 18; c++)
                {
                    scene.AddObstacle(new Obstacle(ObstacleType.Water, c, r));
                }
            }

            return scene;
        }

        private static Scene GenerateScene10()
        {
            var scene = new Scene("6 skeletons vs 3 knights (automatic)", 8, 8);
            scene.AddCreature(new Creature(_skeleton, true), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 7, 0);
            scene.AddCreature(new Creature(_knight, false) {IsAutomatic = true, HitPoints = 15}, 3, 3);
            scene.AddCreature(new Creature(_knight, false) {IsAutomatic = true}, 4, 3);
            scene.AddCreature(new Creature(_skeleton, true), 0, 7);
            scene.AddCreature(new Creature(_skeleton, true), 7, 7);
            scene.AddCreature(new Creature(_knight, false) {IsAutomatic = true}, 6, 3);
            scene.AddCreature(new Creature(_skeleton, true), 0, 4);
            scene.AddCreature(new Creature(_skeleton, true), 7, 4);

            return scene;
        }

        private static Scene GenerateScene11()
        {
            var scene = new Scene("16 skeletons vs 2 behirs", 8, 8);

            for (var r = 6; r < 8; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    scene.AddCreature(new Creature(_skeleton, false), c, r);
                }
            }

            scene.AddCreature(new Creature(_behir, true), 3, 0);
            scene.AddCreature(new Creature(_behir, true), 4, 0);

            return scene;
        }

        private static Scene GenerateScene12()
        {
            var scene = new Scene("humans vs goblins", 10, 10);

            for (var c = 3; c < 7; c++)
            {
                scene.AddCreature(new Creature(_goblinArcher, true), c, 0);
            }

            for (var r = 1; r < 2; r++)
            {
                for (var c = 2; c < 8; c++)
                {
                    scene.AddCreature(new Creature(_goblin, true), c, r);
                }
            }

            for (var r = 4; r < 6; r++)
            {
                for (var c = 0; c < 10; c++)
                {
                    if (c < 4 || c > 5)
                    {
                        scene.AddObstacle(new Obstacle(ObstacleType.Water, c, r));
                    }
                }
            }

            //scene.AddCreature(new Creature(_knight, true), 2, 0);
            //scene.AddCreature(new Creature(_knight, true), 7, 0);

            //scene.AddCreature(new Creature(_archer, false), 1, 9);
            //scene.AddCreature(new Creature(_archer, false), 2, 9);
            scene.AddCreature(new Creature(_archer, false), 3, 9);
            scene.AddCreature(new Creature(_archer, false), 4, 9);
            scene.AddCreature(new Creature(_archer, false), 5, 9);
            scene.AddCreature(new Creature(_archer, false), 6, 9);
            scene.AddCreature(new Creature(_knight, false), 7, 9);
            scene.AddCreature(new Creature(_knight, false), 8, 9);
            //scene.AddCreature(new Creature(_knight, false), 9, 9);

            return scene;
        }

        private static Scene GenerateScene13()
        {
            var scene = new Scene("1 Archer against 12 goblins", 5, 10);

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 4));

            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 1, 1);
            scene.AddCreature(new Creature(_goblin, true), 0, 0);
            scene.AddCreature(new Creature(_goblin, true), 1, 0);
            scene.AddCreature(new Creature(_goblin, true), 2, 0);
            scene.AddCreature(new Creature(_goblin, true), 3, 0);
            scene.AddCreature(new Creature(_goblin, true), 4, 0);
            scene.AddCreature(new Creature(_goblin, true), 5, 0);
            //scene.AddCreature(new Creature(_goblin, true), 6, 0);
            //scene.AddCreature(new Creature(_goblin, true), 7, 0);
            scene.AddCreature(new Creature(_goblin, true), 0, 2);
            scene.AddCreature(new Creature(_goblin, true), 1, 2);
            scene.AddCreature(new Creature(_goblin, true), 2, 2);
            scene.AddCreature(new Creature(_goblin, true), 3, 2);
            scene.AddCreature(new Creature(_goblin, true), 4, 2);
            scene.AddCreature(new Creature(_goblin, true), 5, 2);
            //scene.AddCreature(new Creature(_goblin, true), 6, 2);
            //scene.AddCreature(new Creature(_goblin, true), 7, 2);

            return scene;
        }

        private static Scene GenerateScene14()
        {
            var scene = new Scene("Archers vs goblins", 10, 10);
            scene.AddCreature(new Creature(_archer, false), 4, 4);
            scene.AddCreature(new Creature(_archer, false), 5, 4);
            scene.AddCreature(new Creature(_archer, false), 4, 5);
            scene.AddCreature(new Creature(_archer, false), 5, 5);

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 3));

            for (var r = 0; r < 10; r++)
            {
                for (var c = 0; c < 10; c++)
                {
                    if (r < 3 || r > 6 || c < 3 || c > 6)
                    {
                        scene.AddCreature(new Creature(_goblin, true), c, r);
                    }
                }
            }

            return scene;
        }

        private static Scene GenerateScene15()
        {
            var scene = new Scene("3 archers vs 1 goblins archer", 10, 10);

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 4, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 5, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 6, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 7, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 8, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 9, 3));

            scene.AddCreature(new Creature(_goblinArcher, true), 3, 1);
            scene.AddCreature(new Creature(_archer, false), 3, 5);
            scene.AddCreature(new Creature(_archer, false), 4, 5);
            scene.AddCreature(new Creature(_archer, false), 5, 5);

            //scene.AddCreature(new Creature(_goblinArcher, true), 9, 0);
            //scene.AddCreature(new Creature(_archer, false), 8, 1);
            //scene.AddCreature(new Creature(_archer, false), 6, 1);
            //scene.AddCreature(new Creature(_archer, false), 4, 1);

            return scene;
        }

        private static Scene GenerateScene16()
        {
            var scene = new Scene("humans vs goblins 2", 12, 12);

            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 4, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 5, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 6, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 7, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 8, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 9, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 10, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 11, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 4, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 5, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 6, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 7, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 8, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 9, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 10, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 11, 4));

            scene.AddCreature(new Creature(_goblinArcher, true), 6, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 7, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 8, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 9, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 10, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 11, 0);
            scene.AddCreature(new Creature(_goblinArcher, true), 6, 1);
            scene.AddCreature(new Creature(_goblinArcher, true), 7, 1);
            scene.AddCreature(new Creature(_goblinArcher, true), 8, 1);
            scene.AddCreature(new Creature(_goblinArcher, true), 9, 1);
            scene.AddCreature(new Creature(_goblinArcher, true), 10, 1);
            scene.AddCreature(new Creature(_goblinArcher, true), 11, 1);

            //scene.AddCreature(new Creature(_goblin, true), 1, 0);
            scene.AddCreature(new Creature(_goblin, true), 2, 0);
            scene.AddCreature(new Creature(_goblin, true), 3, 0);
            scene.AddCreature(new Creature(_goblin, true), 4, 0);
            scene.AddCreature(new Creature(_goblin, true), 5, 0);
            //scene.AddCreature(new Creature(_goblin, true), 1, 1);
            scene.AddCreature(new Creature(_goblin, true), 2, 1);
            scene.AddCreature(new Creature(_goblin, true), 3, 1);
            scene.AddCreature(new Creature(_goblin, true), 4, 1);
            scene.AddCreature(new Creature(_goblin, true), 5, 1);

            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 11, 11);
            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 10, 11);
            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 9, 11);
            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 11, 10);
            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 10, 10);
            scene.AddCreature(new Creature(_archer, false) { IsAutomatic = true }, 9, 10);

            scene.AddCreature(new Creature(_knight, false) { IsAutomatic = true }, 8, 11);
            scene.AddCreature(new Creature(_knight, false) { IsAutomatic = true }, 8, 10);
            scene.AddCreature(new Creature(_knight, false) { IsAutomatic = true }, 7, 11);
            scene.AddCreature(new Creature(_knight, false) { IsAutomatic = true }, 7, 10);

            return scene;
        }

        private static Scene GenerateScene17()
        {
            var scene = new Scene("1 knight - 1 x 1", 1, 1);
            scene.AddCreature(new Creature(_knight, false), 0, 0);

            return scene;
        }

        private static Scene GenerateScene18()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 2 x 2", 2, 2);
            //scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 1, 1);

            return scene;
        }

        private static Scene GenerateScene19()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 3 x 3", 3, 3);
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 2, 2);

            return scene;
        }

        private static Scene GenerateScene20()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 4 x 4", 4, 4);
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 3, 3);

            return scene;
        }

        private static Scene GenerateScene21()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 5 x 5", 5, 5);
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 4, 4);

            return scene;
        }

        private static Scene GenerateScene22()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 3 x 5", 3, 5);
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 2, 4);

            return scene;
        }

        private static Scene GenerateScene23()
        {
            var scene = new Scene("1 skeleton vs 1 knight - 5 x 3", 5, 3);
            scene.AddCreature(new Creature(_knight, false), 0, 0);
            scene.AddCreature(new Creature(_skeleton, true), 4, 2);

            return scene;
        }
    }
}