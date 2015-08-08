# AltspaceVR Unity Cursor - Xin Ning  

## Brief  
For Assignment 1, I achieved the 3D cursor as described movement behaviour and shader   
For Assignment 2, I developed a multiplayer ship sailing experience based on the 3D cursor interaction.  

Build Location  
  - Assignment 1 - 3D Cursor  
	/Builds/AltspaceCursor.zip    
  - Assignment 2 - Pirate Ocean  
	/Builds/PirateOcean.zip  

Scene and Script Location  
  Assignment 1 - 3D Cursor  
  - Scene  
      /Main.Scene  
  - Scripts  
	  - Modified 3D Cursor Script  
		/Altspace/Scripts/SphericalCursorModule.cs  
	  - Custom shader with Self-Illumn and always on top  
		/Altspace/Shaders/CursorShader.shader  
	
Assignment  2 - Pirate Ocean  
  - Scene  
		/PirateShip/Scene/PirateOcean  

- Scripts  
		/PirateShip/Scripts  

		  
		  
# Assignment 2 - Introduction
Beyond the cursor enhancement, I tried to come up with a unique experience that VR social platform such as AltspaceVR could deliver, and aimed to develop an interactive experience to emphasize such unique VR social experience.  
- The goal for a social platform is to hangout and get relaxed interactions and communications with friends
- the unique experience for VR is the ability to bring user into immersive environments that is impossible in real life. 
Thus, I tried combine these two features and developed this multiplayer sailing experience. Players can dive into beautiful ocean environment and have some fun working together to get the ship sail, or even get some pirate ship blowed up!

Experience Goal
- Just hangout with friends, have some fun sailing the ship, enjoy the ocean and blow some pirate ship away!

Cursor Enhancement
- Player is able to select and use objects, interact with them in network multiplayer environment


## Control Instruction
- Start a Game  
  To Start a game, do either  
	- Start the application, and click Server to host a server. Beware that the port number is required for other player to join the server  
	
  	- Or put in a hosted server's IP address and port, then click Client to join
    
- Join as an Avatar  
  Once hosted or joined successfully, click on Join to join the world as an avatar.
  
- Avatar Control  
  WASD        : Move  
  Mouse Move  : 3D Cursor movement  
  Shift+ Mouse: Look around  
  
  Left Click  : Interact with Highlighted Interactable Object. This will lock your movement temporalily  
  Right Click : Release from interaction of highlighter object  
  
- Interactable Object Instruction  
  Sail Rope   : Control the speed of the ship. W/S to increase/decrease speed.  
  Sail Wheel  : Control the steer direction of the ship. A/D to steer left/right.  
  Cannon      : Fire cannon to destroy hostile pirate ship! Mouse movement to aim, Mouse Left Click to shoot.   


## Notes on Design Choices
- Referencing to the social objective, the control of the ship (sail rope and wheel) and cannons are intentionally scattered around the deck to encourage cooperation and communication between players. One player can not do it all by himself at the same time.  
- The sailing rope and steer wheel was originally interacted by 3D cursor dragging, as a cursor enhancement, yet turn out to be quite awful to manipulate. So I decided to made the trade-off and and go back to original WASD control.  
- Cannons are designed with no aiming support, to both fit the genre, and to let player enjoy the happy time of bullet hail shooting without careful aiming.  


## Noticed Issue  
- Jiggling for client users when ship turns.  
  This is caused due to the current ship movement synchronization algorithm, it's interpolate prediction only takes moving velocity into account. The left over angular velocity caused significant error and lead to jiggled warp correction.  
  If there is more time to implement the prediction with angular velocity, this problem might be solved.  
  
- Player slip a bit when ship start to sail or stop.  
  This was caused by the ship inertia, players are physics objects stand on the ship rather than fixed to the ship. This is not essentially a bug or issue, it created a level of feeling that player is stand on a sailing ship. Yet this might cause dizziness if player is wearing Oculus Rift, in such case, we might want to disable this feature and fixed positions.  

  
  
# Acknowledgements
Lists of Assets I used for Assignment 2 - Pirate Ocean

## Unity Standard Assets  
  - Particle Effects  
  - Water(Pro)  
  - Skybox  
  - Detonator Explosion Pack  
  
## Other Models  
  - Ship Model from cgmodel.cn  
  - Robot Model from Unity Asset Store Robot Kyle  
  - Island Terrain Pack from Unity Asset Store  
  - Kandol Free Particle Pack from Unity Asset Store  
  
## Scripts
  - Network Synchronization Code from my past project, reused roughly 50%  
  - Silhouette Outline Shader from Unity Wiki, modified as specular outline shader.  
  
## Sound
  - All free sound effects from http://soundbible.com/  
  - Background Music from League of Legend - BILGEWATER GANGPLANK Login Theme  
  
