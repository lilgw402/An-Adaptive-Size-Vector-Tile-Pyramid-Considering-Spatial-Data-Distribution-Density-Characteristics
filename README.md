# An-Adaptive-Size-Vector-Tile-Pyramid-Considering-Spatial-Data-Distribution-Density-Characteristics


Development platform： ArcEngine 10.7.1
Web server-side framework： Express 4.16.4

we propose a novel three-step method for dynamically adjusting vector tile sizes based on spatial data distribution density during construction. Firstly, we generalize the raw data into a multi-resolution vector dataset encompassing varying levels of detail (LOD). Subsequently, we construct adaptive-size vector tiles for each level of the dataset, matching the specific spatial distribution density characteristics. Finally, we encode these differently sized vector tiles using geohash technology to facilitate spatial indexing and internet transmission. 
