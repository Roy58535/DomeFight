# 1. Title Page
#### Game Name: Dome Fight
#### Tag Line: "Didn't see that coming, huh?"
#### Team
- Lead Designer & Programmer: Amano Teru / Roy Yang

# 2. Game Overview
#### Purpose of the game
Dome Fight is a special planetarium-based arena brawler dedicated to funny and chaotic offline multiplayer combat offline in a 360-degree dome planetarium. The game uses the planetarium dome as the arena instead of the traditional flat computer/television screen, creating unexpected chaotic combat effects. 

#### Intended Use
Dome Fight is intended as a prototype for dome planetarium display events and an experiment of implementing video games in planetariums.

#### Justification for the Use
A brawler game using the dome as the arena can bring very different experiences with conventional brawlers. This game is a salute to Stick Fight, a 2d multiplayer minimalist brawler in which players engage in chaotic combat with map mechanisms and randomly obtained weapons. The dome arena allows the players to go 360 degrees around, projectiles to fly across the top of the dome and hit another player on the other side, players to fly across if they gain enough momentum, etc. It brings extra chaos to the party and can be very entertaining. 

#### Target audience
Dome Fight is mainly targeted at players who enjoy local sofa party games, particularly teenagers or young adults bringing their friends to the dome.

# 3. Gameplay
#### Objectives
Players fight to eliminate opponents by using guns or melee weapons and environmental tactics to gain points. Whoever scores the targeted number of scores wins.




# 4. Mechanics

#### Game Progression
##### demo Version
- Scoring
	- 2-4 Players can choose the total number of scores needed to win.
	- The players fight each other, and the last one that survives gain 1 score.
	- Whoever is the first to reach the chosen winning score, wins the game. (This guarantees 1 winner)
- The Arena
	- The arena is the full dome - essentially the surface of a hemisphere. The players in reality will stand/sit inside the dome looking up, and the characters in the game will move on the surface of the hemisphere. 
	- The arena wraps around the hemisphere - a sufficiently large force could send an entity up and over and land on the other side.
- Maps
	- A random map is chosen from the map pool for each score.
- Weapons
	- The player starts with a default weapon that they can choose before the start of the entire game
		- Pistol, Katana, SMG, Super Shorty
	- Weapons will be randomly generated throughout the map periodically.

#### Player Actions 
- Combat
	- Primary Weapon
		- RT - fire
		- RB - reload
		- R joystick - aim
		- LT - block bullets if weapon permits, with cooldown
	- Melee
		- Y - melee attack
- Movement
	- L joystick - Left and Right
	- A - jump, double jump, wall jump
