# Order in layer for every visual layer in levels
For the sake of the confortability, OiL will be an abreviation of Order in Layer
Starting from the point of view of the Camera (from closer to further):
1. Tutorial Canvas (as it is the only Canvas Overlay it will stay OiL = 0)
2. Curtain OiL = 300
3. Confeti OiL (inside Renderer) = 200
4. Interactable Canvas (that with time control, solve check, next level window, etc - Camera Canvas) OiL = 100
5. Road Users (with a Sorting Group for comfortability) OiL = 0
6. Screen Background (Still not implemented due to lack of time) OiL = -100
7. Road Background OiL = -200
8. Office Background (as Screen BG, not implemented) OiL = -300

# Why Curtain is not inside a Canvas?
Because Masks are still not prepared to work as intended inside a Canvas, since for a Mask to be applied to an Image, this latter must be child of the mask, and in this case we needed the mask move around the image and not the other way around

# How to create a new Level
It is highly recommended to duplicate one of the already developed levels and
perform the following changes:

1. Review, and if so, edit the **LevelManager** GameObject, it has some of the most important preferences to perform the level, including the duration of the level loop.
2. **TrafficAreaUI** Here is where the TrafficLights are configurated. Use **TrafficLight** Prefab for every crossroad and set the times in the following order **red, green and yellow**, then you can set a time offset so the TrafficLight begins in a different time. e.g we configure the traffic light so that red takes 1 second, then green 2 seconds and yellow 1 second,  but we want the TrafficLight to start at yellow, then we must set 3 in the offset (red + green times).

3. **TrafficAreas**. This is somehow decided by the image used as background.
  2.0. Must have in its root a TrafficLightReference component pointing to the corresponding TrafficLightController in Canvas   
  2.1. It contains all the **colliders** of the crossroad, i.e. 4 **stop areas** just before reaching the center of the crossroads (used for the road users to stop), the **center** of the crossroad (used to determine the speed (if they got stuck in middle, they must run)) and **anticipation areas** areas before the stop to react before reaching the area.
  Remember that every area must be on a different GameObject and contain a TrafficArea with the direction of the area in question.

  2.2. Exactly one traffic light that will control how the road users interact with the crossroad.

  2.3. A **bezier spline** for every direction in the crossroad  
4. Decide how many actors will there interact (pedestrians and vehicles), you can add them from prefabs or create variants. In the root we have a BezierWalkerWithSpeed, we need to assign here a BezierSpline to define their direction (not need to change the speed here, it will be overwritten afterwards)
Then you can access the GameObject child where you can edit the speed and other options, which can be found in the **PedestrianController or VehicleController**



## Less important changes
5. You can change music, located in GameObject Technical > Jukebox
