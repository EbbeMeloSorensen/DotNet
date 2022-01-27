// See https://aka.ms/new-console-template for more information

using Craft.Math;

Console.WriteLine("Fun with Craft.Math");

var vector1 = new Vector2D(1, 2);
var vector2 = new Vector2D(3, 1);
var vector3 = vector1 + vector2;
Console.WriteLine($"Length: {vector3.Length}");

