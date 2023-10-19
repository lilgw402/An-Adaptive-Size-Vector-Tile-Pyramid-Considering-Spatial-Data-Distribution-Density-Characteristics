# An-Adaptive-Size-Vector-Tile-Pyramid-Considering-Spatial-Data-Distribution-Density-Characteristics


Development platform： ArcEngine 10.7.1
Web server-side framework： Express 4.16.4

we propose a novel three-step method for dynamically adjusting tile sizes based on spatial data distribution density during construction. Firstly, we generalize the raw data into a multi-resolution vector dataset encompassing varying levels of detail (LOD). Subsequently, for each level within the dataset, we employ a quadtree-based approach to construct adaptive-size vector tiles that align with the specific spatial distribution density characteristics. Finally, we encode these differently-sized vector tiles using Geohash technology to facilitate spatial indexing and internet transmission. Experimental results validate the effectiveness of our approach, showcasing its ability to significantly reduce the overall number of tiles, achieve balanced data volumes between tiles, and improve the internet transmission efficiency of tiles. 
