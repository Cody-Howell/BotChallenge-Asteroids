# BotChallenge-Asteroids

This is Bot Challenge 1 (of.. some amount)! There is a global Server app that runs the game, 
a console to program your bot in, and a React app (served through the default route in the Server app) to play as a human. 

## Running the Game

Navigate to BC-Asteroids.API. This is the server for the game as well as hosting the React app. 

Start up with `dotnet run`. At the top will be a line that looks something like this: 

```
Admin Key: %zmi!+|-^:A+0?d6xE+](4t6*Tn1yFV}
```

Copy the key. Open the app in your browser (just two lines below this key is where it's listening). It will open up the Admin console. Paste in the key to the input value, then click the button to create a game. The route to any game is just the root and the id; for example, `http://localhost:5038/1` navigates to the first game. There won't be anything there if there's no game hosted. 

Click register in the top left. It should turn green. Navigate to the admin console on another tab and go down to the table, then click Start. The game should now be running in real time. 

I use WebSockets to get game updates out, so it should feel quite smooth. For the human, the keys are WASD to move, Space to fire, and B to brake (which quickly stops you in space). 

It's **important** to note that on the visualization, the hitboxes are not very accurate. They are originate from the top left, so while it looks empty up there, that's actually where they are. This is not a problem for the console or your bots.  

## What the game is

This is a recreation of Asteroids. You are given a tiny little ship in an increasingly full screen of asteroids. 

- Players spawn in the exact middle of the play area, overlapping each other at the start of the game. 
- Asteroids spawn at the edge of the screen going in a random direction.

I use my own physics library to run the calculations, which handles overlaps (`Circle2D`), vectors (`Vector2D`), points (`Point2D`), and rotations (`Rotation2D`). These all have a variety of methods to make some calculations easier. I hope these make it easier to implement your algorithms (especially the Rotations; I've known how much of a nightmare they are for nearly 2 years :P). 

Your character moves in space. There is no friction, so you won't slow down unless you accelerate in the opposite direction of your movement or use the Brake command. 

## Playing the game

### Score

The person with the max score wins. There are a few ways to get points: 

- You get 1 point for every tick you're alive (there are 30 ticks per second, which is how often the game updates)
- Shooting other players gets you 100 points
- Shooting asteroids give you 50 * size of the asteroid. 
    - Largest will give 50. 
    - Medium gives 100. 
    - Small gives 150. 
- You can also run into them and get the same number of points (they will also break apart). However, this reduces your health. 

### Health

You get 100 health to start. There are only two ways to lose health: 

- Hitting an asteroid removes 34 health (which is just over a third, so you have at most 3 collisions before death)
    - This also immediately stops your player in space. 
- Getting hit by the bullet of another player
    - This removes 1 health, but is more effective for points (if you can even do it; it's quite hard). 

After getting hit, you are given 2 seconds (60 ticks) of intangibility, where you can't interact with asteroids. After that time runs out, you are vulnerable again.

### Configuration

The configuration file is in the API (`config.json`), and you can adjust it however you'd like. The below is the default configuration, and likely what we'll use for the competition. It's generally made to feel natural to human players and give a lot of space to play around with. 

Also, you may notice that there's a _ton_ of asteroids. I expect around 5 players to fit into a visible field pretty well, and we'll just see how it goes. I do want games to end eventually, and that's why there's so many. If you want to reduce the number of asteroids, increase the number for `asteroidFrequency`. 

```json
{
  "player": {
    "topSpeed": 20,
    "rotationSpeed": 4,
    "movementSpeed": 0.2,
    "radius": 8, 
    "fireSpeed": 30
  },
  "bullet": {
    "defaultSpeed": 30,
    "radius": 2, 
    "duration": 120
  },
  "asteroid": {
    "smallestRadius": 10
  },
  "game": {
    "asteroidFrequency": 75
  }
}
```

All the above are listed in ticks, since ticks are time-independent. Play with it and see how it changes! (I believe everything is correct so it should reflect on the UI). 

## Using the console

The console will ask for what game ID to reference, then the URL to go to. After it's connected, it will run through a while loop, read the game state as it comes in and converts it to actual objects to read from, and send the response back to the server to update your commands. 

If you somehow send two updates in one tick (the current console won't do that, but if you code something else), then it will keep the most updated commands. 

### Coding your bot

Go to the Console app in the solution. This has all the sample code and has a spot for you to put your logic. There are lists of game objects there to decide from, and your bot can take any of the following updates every tick: 

- Acceleration / Brake
    - The forward and backward movement between 1 and -1 of the movement speed in the configuration. For braking, it slows your ships velocity by dividing by 1.2 every time.
- Left / Right
    - Left and right movement of your ship. For human players, these are full-throttle points. 
    - For bots, you have the POINTAT command which will automatically take the largest step towards that direction, or point directly at it if it's within the rotation speed allowance. 
- Fire
    - You can fire a bullet every # of ticks as specified in the configuration. Once it hits 0, you can fire whenever you want (firing is not on a clock, just a cooldown). You can send a fire command every tick, but it will only activate when it's allowed. 

For your coding, I've provided a static class to call to ensure your commands match what the server expects. This is the `AvailableMoves` class, and you can see the Intellisense of what moves you have available to you. If one takes in a parameter, that parameter is in the method and will handle the string version of it for you. 

All you need to do is fill your `moves` list (using `moves.Add(AvailableMoves.___()))`) and then send that off to the server, and it will perform that action on the next tick. 

If you don't provide a response to the server, it will use whatever it last had as your commands. So if you want to change nothing, you can choose not to send a response and it will take the most recent action you sent for it. 

## Command reads

Some commands are logical opposite from each other. To handle this, I read the from the end of the list and take the first actions that don't interfere with each other. After that, interfering actions are ignored.

If your bot sends commands to turn left, turn right, and point at (0,0), it will take the latest action in the list and perform that. 

Similarly, if you ask it to accelerate forward, backward, and brake, it will only take the latest action in the last. 

Fire does not interfere with anything. 

---

The server will send out a game state every 33 ms, so your logic should perform and send a response in that time. There is no penalty for being late, as you can choose not to send an action at all, but your logic will be 33 ms behind. Which may or may not be an issue.  
