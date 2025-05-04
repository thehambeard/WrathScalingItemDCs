# ScalingItemDCs

This mod is SAVE SAFE.  It can be added and removed at anytime and will not break your game.

Adds a few options to increase the base DCs of equipment.

Options:
  * Flat.  This will increase all DCs by a flat set amount.  If you want all DCs 10 higher then set amount to 10.
  * Percentage.  This will increase all DCs by a percentage.  If you want all DCs 10% higher then set amount to .1.
  * Diminished Returns.  This is a more advanced setting.  The formula to calculate the amount is f(x) = (x + b) / (x * a) + c). X is the DC and will create a modifier based on it and the a, b, c parameters.
    The goal is to give lower DCs a bigger boost and the higher DCs a smaller amount.  I would suggest using a graphing calculator to fine tune your parameters if there is something specific you are looking for.
    
There are a few presets that can be picked from or you can set your own.  It is up to you.
Below each setting is a chart to preview your changes XX -> YY. XX is the base DC, and YY is the modified DC.
Save button will save any changes you have made and set the preset.  IF YOU CHANGE WHAT PRESET YOU ARE USING YOU STILL MUST HIT SAVE.

If Modders want to add items to list call WrathScalingItemDCs.ScalingDC.ScalingDCAPI.AddItem(BlueprintItemEquipment blueprintItemEquipment) from your mod.
