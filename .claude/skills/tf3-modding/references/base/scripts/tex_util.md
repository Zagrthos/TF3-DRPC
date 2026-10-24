# scripts.tex_util

Definition file: `base/tealdef/scripts/tex_util.d.tl` (in the game folder).

Types of `/scripts/tex_util.tl` (`TexUtil`), helpers that build texture descriptions with common filter and wrap settings.

## TexUtil (record)

```teal
global record TexUtil
```
Load: `local tu = require "::/scripts/tex_util.tl" as TexUtil`

Texture description helpers returned by `ug_require "/scripts/tex_util.tl"`.

- `makeMaterialIndexTexture : function(fileName : string, wrapS : WrapMode, wrapT : WrapMode) : TextureDesc`  (call as `tu.makeMaterialIndexTexture(…)`): Create a material index texture description.
    - param `fileName`: path to the texture file
    - param `wrapS`: horizontal wrap mode
    - param `wrapT`: vertical wrap mode
    - returns: texture description
    - used 792x in the base game, e.g. `base/content/assets/rocks/shared/shared/gtex/rock_ground_large.gtex.lua:5`: `texture = tu.makeMaterialIndexTexture("tex/rock_ground.tga", "CLAMP_TO_EDGE", "CLAMP_TO_EDGE"),`
- `makeTextureLinearClamp : function(fileName : string, compressionAllowed : boolean, scaleDownAllowed? : boolean, redGreen? : boolean, texType? : TexType) : TextureDesc`  (call as `tu.makeTextureLinearClamp(…)`): Create a linear-filtered, clamped texture description.
    - param `fileName`: path to the texture file
    - param `compressionAllowed`: whether compression is allowed
    - param `scaleDownAllowed`: whether the texture may be scaled down
    - param `redGreen`: whether to use only red and green channels
    - param `texType`: texture dimensionality type
    - returns: texture description
- `makeTextureMipmapRepeat : function(fileName : string, compressionAllowed : boolean, scaleDownAllowed? : boolean, redGreen? : boolean, texType? : TexType) : TextureDesc`  (call as `tu.makeTextureMipmapRepeat(…)`): Create a mipmapped, repeating texture description.
    - param `fileName`: path to the texture file
    - param `compressionAllowed`: whether compression is allowed
    - param `scaleDownAllowed`: whether the texture may be scaled down
    - param `redGreen`: whether to use only red and green channels
    - param `texType`: texture dimensionality type
    - returns: texture description
- `makeTextureMipmapClamp : function(fileName : string, compressionAllowed : boolean, scaleDownAllowed? : boolean, redGreen? : boolean, texType? : TexType) : TextureDesc`  (call as `tu.makeTextureMipmapClamp(…)`): Create a mipmapped, clamped texture description.
    - param `fileName`: path to the texture file
    - param `compressionAllowed`: whether compression is allowed
    - param `scaleDownAllowed`: whether the texture may be scaled down
    - param `redGreen`: whether to use only red and green channels
    - param `texType`: texture dimensionality type
    - returns: texture description
- `makeTextureMipmapClampVertical : function(fileName : string, compressionAllowed : boolean, scaleDownAllowed? : boolean, redGreen? : boolean, texType? : TexType) : TextureDesc`  (call as `tu.makeTextureMipmapClampVertical(…)`): Create a mipmapped, vertically-clamped texture description.
    - param `fileName`: path to the texture file
    - param `compressionAllowed`: whether compression is allowed
    - param `scaleDownAllowed`: whether the texture may be scaled down
    - param `redGreen`: whether to use only red and green channels
    - param `texType`: texture dimensionality type
    - returns: texture description

### TexUtil.TexType (enum)

```teal
enum TexType
```

Texture dimensionality.

- `"TWOD"`: 2D texture
- `"CUBE_MAP"`: Cube map texture
- `"TWOD_ARRAY"`: 2D array texture
- `"THREED"`: 3D texture

### TexUtil.MagFilterMode (enum)

```teal
enum MagFilterMode
```

Magnification filter.

- `"NEAREST"`: Nearest-neighbor magnification
- `"LINEAR"`: Linear magnification

### TexUtil.MinFilterMode (enum)

```teal
enum MinFilterMode
```

Minification filter, optionally with mipmaps.

- `"NEAREST"`: Nearest-neighbor minification
- `"LINEAR"`: Linear minification
- `"LINEAR_MIPMAP_LINEAR"`: Linear minification with linear mipmap interpolation
- `"LINEAR_MIPMAP_NEAREST"`: Linear minification with nearest mipmap selection
- `"NEAREST_MIPMAP_NEAREST"`: Nearest-neighbor minification with nearest mipmap selection

### TexUtil.WrapMode (enum)

```teal
enum WrapMode
```

Texture coordinate wrapping.

- `"REPEAT"`: Repeat the texture
- `"CLAMP_TO_EDGE"`: Clamp to the edge of the texture
- `"MIRROR_REPEAT"`: Mirror and repeat the texture

### TexUtil.TextureDesc (record)

```teal
record TextureDesc
```

Description of one texture: file, filtering, wrapping and compression options.

- `type : TexType`: Texture dimensionality type
- `fileName : string`: Path to the texture file
- `magFilter : MagFilterMode`: Magnification filter mode
- `minFilter : MinFilterMode`: Minification filter mode
- `wrapS : WrapMode`: Horizontal (S) wrap mode
- `wrapT : WrapMode`: Vertical (T) wrap mode
- `mipmapAlphaScale : number`: Alpha scale factor applied during mipmap generation
- `compressionAllowed : boolean`: Whether compression is allowed for this texture
- `redGreen : boolean`: Whether to use only the red and green channels
- `scaleDownAllowed : boolean`: Whether the texture may be scaled down
