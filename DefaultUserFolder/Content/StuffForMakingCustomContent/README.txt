This file contains a bunch of useful notes for making custom content.
Some are actually crucial to functionality (cause I'm too lazy to make better code) so please do read through them.

1. You will notice some images provided in here. These are stuff for you to work off of to make things easier.
	You don't have to use them if you don't want to though.

2. The Sprite Sheet Template comes with heads on it.
	The cosmetic attributes of characters (hair, eyes, mouth) are designed on it and so only really work with it.
	You have the option of using your own head, but these elements may not fit.
	In such a case, set the origin coordinates for them in the charcard editor to 1792 or greater (size of a charcard sprite).
	You can also move the present heads if you want, positioning doesn't really matter so long as you set the coords to fit.

3. Character Card Sprites are also hard-coded.
	This is why the template is provided, so please do make use of it for your own ease.
	You may want to remove the lines between each section once finished, sometimes they show up in-game for some reason.

4. If you want, you can replace the spritesheets for the cosmetic attributes with your own.
	Make sure to use the same sizes for each element though, since that part's hardcoded.
	To this end, templates are provided in this folder. I'd recommend using them since they come with borders.
	You can choose between the one with heads or the one without.
	If you use the one without, character card spritesheets designed to work with the default hairs won't work with them anymore.
	This does allow you to use your own (probably better) head designs though, which is nice.
	You can find the sprite sheets to replace them in the default content folder.
	I removed the borders from the actual ones for the same reason I recommend removing them on character card sprites.
	Hair template is same for both front and back since their sizes are the same.

5. Battle arts and combat card arts, unlike the previous two things, can be any size.
	However, they'll be adjusted, so try to keep them in proportion to prevent stretching.
	Both are 60 wide and 37.5 tall, giving a ratio of 1:0.625

6. Conditional text can be overridden manually in the Dontxt files.
	There is a property called something along the lines of "ConditionalDescOverride" which, if set,
	will be used for it instead of the automatic conversion.

7. You can make custom status effects, but you have to just manually add them to Dontxt files.
	You can use existing status effects in the default content folder as a basis, they should be easy enough to figure out.

8. You can't literally make custom dice, but you can use dice type 9 which does nothing along with an on hit conditional instead.

9. You can replace all the music and sounds effects.
	You can have as many or as few songs as you want, as long as they a prefixed with either MENU_ or BATTLE_
	(depending on which you want them to play in) and somewhere within the content folder. You can use ogg or wav, but wav can have issues.
	To replace the SFX, they must have the same name as the original ones but they can be anywhere within the content folder.
	You should remove the original SFX files if you replace them, as they may override your own if the system loads them after rather than before.