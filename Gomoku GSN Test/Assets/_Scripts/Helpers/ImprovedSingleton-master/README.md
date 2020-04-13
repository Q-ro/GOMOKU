# UNITY SINGLETON
A "rub my back" approach to a singleton class to be used on unity projects.

## Introduction
An inheritable class that defines a singleton base pattern to be implemented by other classes, based on the implementation described in the [Unity wiki](https://wiki.unity3d.com/index.php/Singleton), with slight modifications by Andres Mrad (Q-ro).

## Why ?
The aforementioned definition of the pattern makes it really cumbersome to test the game when a singleton instance needs to be present on more than one scene but you are testing scenes one by one, forcing you to either run the game "from the very first scene" where the singleton instance was created, or, deleting every singleton instance when building the game or testing from scenes where the singleton was instantiated and moving to another scene with another instantiation of the same singleton. While this implementation may not address everything that could be wrong with the aforementioned, is good enough for me and addresses all the issues I have encountered so far while using it.

## Lincense
The original code is lincensed as (and by extention this modification) Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0), meaning you are free to :

- Share — copy and redistribute the material in any medium or format
- Adapt — remix, transform, and build upon the material for any purpose, even commercially.
- This license is acceptable for Free Cultural Works.
- The licensor cannot revoke these freedoms as long as you follow the license terms.