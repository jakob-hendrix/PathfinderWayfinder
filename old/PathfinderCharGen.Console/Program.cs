// See https://aka.ms/new-console-template for more information

using PathfinderCharGen.RulesEngine;

// TODO: use Spectre.Console to create tool to edit state

CharacterState characterState = new CharacterState();
Console.WriteLine($"Character name is {characterState.CurrentCharacter.Name}.");
Console.ReadLine();