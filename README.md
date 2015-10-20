﻿# Monogame custom content pipeline tools

This small library adds `SharedContentManager` and `RefCountedContentManager` classes to Monogame. 


### SharedContentManager

This is a content manager which can be used to preserve shared assets: 
an asset that is loaded for Level #1 and re-used in Level #2 can be kept in memory instead of being destroyed and reloaded. 
Just load Level #2 assets first, then unload Level #1 assets.


#### Design limitations

All shared content managers must have the same root path and service provider.


### RefCountedContentManager

Internally, `SharedContentManager` uses `RefCountedContentManager`, which can also be used separately if desired. 
As one can guess, `RefCountedContentManager` implements reference counting for assets. 
Each call to `Load<T>` increases asset refcount, each call to `Unload(assetName)` decreases asset refcount. 
Asset will be released when its refcount reaches 0 or when `Unload()` will be called, since it releases all assets.


### Thread-safety

All methods for `SharedContentManager` uses `RefCountedContentManager` are "thread-safe" as in they maintain internal state consistently no matter what sequence of individual operations are happening on other threads. 
You still should lock when you need logical consistency maintained across multiple operations in a sequence.


### Loading/unloading for custom asset types without writing pipeline extension libraries

Sometimes you want to quckly add loading/unloading functions for the custom asset types without "properly" extending content pipeline by writing `MyCustomDataTypeReader/Writer` and `MyCustomDataProcessor` classes. 
As an added bonus, with `SharedContentManager` uses `RefCountedContentManager` you can easily add custom load/unload functions.

So let's teach content managers how to load some custom asset type, named [BMFont](https://github.com/EnoughTea/MonoBMFont). 
Since it is a font, it uses another asset, 2D texture with character glyphs. We will have to deal with it as well.


	// Tell content managers how to properly load BMFont by registering a load function:
	CustomAssets.RegisterLoad<BMFont>((content, fontName) => {
        string fontTextureName = FontData.GetTextureNameForFont(fontName);
        var fontTexture = content.Load<Texture2D>(fontTextureName);

        using (var stream = TitleContainer.OpenStream(Path.Combine(Game.ContentRoot, fontName)) {
            var fontDesc =  FontData.Load(stream);
            return new BMFont(fontTexture, fontDesc);
        }
    });

	// Optional unload step to unload linked assets.
	// Since the font asset used a texture with glyphs, let's register a function to unload it:
    CustomAssets.RegisterUnload<BMFont>((content, fontAsset, fontName) => {
        string fontTextureName = FontData.GetTextureNameForFont(fontName);
        content.Unload(fontTextureName);
    });


	// Now SharedContentManager and RefCountedContentManager can load a font:
	game.Content = new SharedContentManager(Game.Services, Game.ContentRoot);
	var bmFont = Content.Load<BMFont>("Fonts\\Calibri22.fnt");    // Calls registered load function internally
	// ... and unload it later:
	Content.Unload("Fonts\\Calibri22.fnt");    // Calls registered unload function internally