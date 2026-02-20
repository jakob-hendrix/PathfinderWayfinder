# Pathfinder Rules

## Character Creation Guide

<https://www.d20pfsrd.com/basics-ability-scores/character-creation/>

1. Buy ability scores
2. choose race
3. choose 1st level class (part of class level page)
4. allocate skill points (part of skill page)
5. choose feats (part of feat page)
6. hit points (part of class level page for the die roll section. Total hp is calculated by state manager)
7. Equipment (base wealth provided by character state - current wealth tracked by player in log)
8. alignment (needs to trigger re-validation)
9. religion
10. PR flavor - physical description, bio,

At this point the character state manager will be able to determine based on all class levels, all gear, all feats, etc

- saves
- BAB
- initiative
- available attacks

## Character State Pages

This structure is taken from years of playing using YAPCG. Might change based on feed back. Each page will have a navigation tab, and affect a different aspect of the underlying character state manager

- summary - act like a character sheet. Location where user can track HP/ temp HP, non-lethal damage
- base class (creation)
- class levels (1-20, stack). Will need to have validation for each class level in case something is changed downstream that removes qualifaction
- feats & traits - allowed entries provided by class levels, with override to allow more entries to account for GM fiat
- skills ranks (give calculated amount, but allow arbitratry entries)
- languages (gained at start and liguistics)
- spells (unlocked by gaining a class level that has spell casting. Each casting class gives its own spell book - unless some weird rule allows stacking progression)
- inventory and equipment
- conditional modifiers
  - added to by gear, feats, traits, etc - there for reference but at some point I'd like to add a tooltip system for each stat and skill
- buffs & conditions (add negative levels, heroism, etc)
- log - add session notes, which give experience and track money changes
- overrides - place to allow the user to layer on a top layer of changes to character stats, for GM fiat, weird attacks, etc
  - ability score
  - AC
  - Save
  - movement (swim, climb, base speed)
  - size (just make your dude huge for fun)
- custom data entry - not stored in underlying json files, but tracked with the character sheet save file

## Class Levels

- BAB
- Saves
- HP
- Skills
- Spells
- Class Features
- Capstone
- Favored Class Bonus
- Archetypes


## Fun Addons

- allow in memory swapping of characters
- item creation calculator
- spell duration tracker - timeline with spell casting rd - feeds into buffs & conditions

## Optional Rule Systems

- Fractional Base Bonuses - <https://www.d20pfsrd.com/gamemastering/other-rules/unchained-rules/fractional-base-bonuses/>
- Gestalt - level up 2 classes at the same time
- Automatic Bonus Progression
- Mythic
