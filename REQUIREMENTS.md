## Mechanics 

### Player 

The player must drive a ship at the bottom of the screen that can move from right to left and shoot (with cooldown) up to eliminate enemy ships. 

It will have 3 lives that will be lost as enemy shots collide with the player. 

There can only be **one** player bullet on screen. He can't shoot again until the current bullet dissapears.

### Enemies 

The enemies will be arranged in the form of a grid at the top of the screen as shown in the figure. They will go down as in the Space Invaders game, from right to left going down one line each time they reach the right or left stop (see references on YouTube). **The less ships remaining, the faster they will move.**

Enemies shoot at random intervals but they can only shoot if they are the last in each column of enemies (that is, those closest to the player in each column). 
They also can **only shoot if no more than 5 enemy bullets exists**. This means, if there are 4 bullets in the screen, only one enemy can shoot, 
and the rest must wait for a bullet to dissapear before shooting.

![example](Images/candy.png)

Each enemy ship will have a color (Blue, Red, Green or Yellow). When a ship of a specific color is eliminated all ships of the same color *adjacent* to its 4 sides will be destroyed as well. 
This is not chained, a limit of 5 enemies is the maximum amount that can be destroyed at once.

Each ship will award 10 points for its destruction, but if they are destroyed in chain their score will be increased using Fibonacci: 

```
1 ship = 1 * 10 = 10 points
2 ships = 2 * 20 = 40 points
3 ships = 3 * 30 = 90 points
4 ships = 4 * 50 = 90 points
5 ships = 5 * 80 = 400 points
N ships = N * Fibonacci(N+1) * 10 points 
```

### Level 

The level will have 4 protections that will be destroyed as the enemy shoots them. Each protector can absorb up to 5 shots before breaking. 

By eliminating all the enemies from one level, you will go to the next level, there is no change from level to level, you should only restart the enemies, the player position, the amount of lives of the player, and the protectors. 

### Menu 

The main menu will have at least the options to play and exit the game. Playing takes you to the first level and resets the score to 0 each time you lose. In the game, you can exit with the escape key to the start menu and start again. 

## Requirements 

- On the main menu, I should see the current highscore and when pressing play, change to the game scene. 
- Every time an enemy ship is destroyed, the score should increase. 
- The enemies color should be randomly picked. No two playthroughs should have the same pattern. 
- When losing, a menu showing me my last score, my current highscore and the buttons: "Restart" and "Go back to main menu" should appear. The buttons should take the user back to the home scene and restart the level. 
- When I press escape, the game should pause, and I should have a menu with the option "Continue" and "Go back to main menu". The buttons should either continue the game or take the user back to the home scene. 
- When I surpass my highscore, this should change in the UI. 
- The enemies speed should be inversely proportional to the amount left (the less enemies, the faster they are). 
- When I surpass my highscore, this should persist,  and I should be able to see it in the UIs when I launch the game again. 
- I should be able to change the initial speed of the enemies, the speed of the bullets and the speed and amount of lives of the player in an easy to modify way (scriptable object, config file, etc)
- Write [automated tests](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html). 

### Bonus points 

- Have some smooth transition between level transitions (fading/effects/be creative!). 
- When surpassing my highscore, a new visual element should appear in the lost screen UI informing me of my achievement (like a text saying "You have a new highscore"). 
- When I'm about to surpass my highscore, a visual element should inform me of it (a different explosion color, a particle effect, etc.). 
- Don't use PlayerPrefs for persistence. 
- Add some details to make the game scene prettier (a color frame, screen shaking when dying, be creative). 
- When restarting the game, don't reload the scene. 