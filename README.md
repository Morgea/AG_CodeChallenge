Adam Morgenstern - Unity Code Challenge  
Unity 2019.1.0f2

PC version executable: bin\exe
PC version zip: bin\zip
Android version installer: bin\apk

Interface

- Double-click center shape to change it to a random color
- Menu button at the top-left opens and closes a shape selection panel and option toggle buttons
- Quit button at the top-right exits the application
- Shape menu panel switches between several possible shapes
- Toggle button panel has several on/off switches (all on by default) for:
  - Turning on/off the treeline background silhouette
  - Turning on/off the atmospheric background particles
  - Turning on/off the particle burst within the shape when it's double-clicked
  - Turning on/off the randomization of shape colors (Off will cycle through a set of predefined colors: white/red/green/blue)
    
  
Scripts
  
- BackgroundGradients: Handles fading the sky/ground background gradients through a series of color sets (day/night cycle)
- Color Randomizer: Handles randomizing the shape color when it's clicked and creating a particle burst each time it's changed
- DoubleClickOMD: Handles OnMouseDown double-click on shapes
- MenuSystem: Handles opening/closing the menus and menu button clicks
- OrientationChange: Zooms the camera out and slides the shape down if in a portrait orientation so menu doesn't cover shapes
- ShapeTracker: Creates the shape meshes based on points and handles the shape change animation
- SlowSpin: Randomizes a slow alternating counter/clockwise spin on the shapes

