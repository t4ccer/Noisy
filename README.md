# Noisy
Noisy is c# library for generating noises. At this moment Noisy implements 3D Perlin Noise(can be used as 2D also), Open Simplex Noise(2D and 3D) and WorleyNoise(2D and 3D).
## installation
To use in Your code just install [t4ccer.Noisy](https://www.nuget.org/packages/t4ccer.Noisy) nuget package.

## Usage
Getting noise value at point:
```csharp
//2D noise
var value = noise.At(x, y);
//3D noise
var value = noise.At(x, y, z);
```

Getting noise value at plane(2d array):
```csharp
var values = noise.At(x, y, width, height, increment);
//or
var values = noise.At(x, y, z, width, height, increment);
```

Getting noise value at line(1d array):
```csharp
var values = noise.At(x, width, increment);
//or
var values = noise.At(x, y, z, width, increment);
```

### Open Simplex Noise 2D
```csharp
var noise = new OpenSimplexNoise2DGenerator();
//or
var noise = new OpenSimplexNoise2DGenerator(seed);
//or
var noise = new OpenSimplexNoise2DGenerator(perm);
```

### Open Simplex Noise 3D
```csharp
var noise = new OpenSimplexNoise3DGenerator();
//or
var noise = new OpenSimplexNoise3DGenerator(seed);
//or
var noise = new OpenSimplexNoise3DGenerator(perm);
```

### Worley Noise 2D
```csharp
var noise = new WorleyNoise2DGenerator(pointCount, n, minPointX, minPointY, maxPointX, maxPointY);
//or
var noise = new WorleyNoise2DGenerator(pointCount, n, minPointX, minPointY, maxPointX, maxPointY, seed);
```

### Worley Noise 3D
```csharp
var noise = new WorleyNoise3DGenerator(pointCount, n, minPointX, minPointY, minPointZ, maxPointX, maxPointY, maxPointZ);
//or
var noise = new WorleyNoise3DGenerator(pointCount, n, minPointX, minPointY, minPointZ, maxPointX, maxPointY, maxPointZ, seed);
```

## Contribution
Feel free to add some features or fix bugs
