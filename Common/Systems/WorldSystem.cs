using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using GuardianClass.Content.Tiles.Trees;

namespace GuardianClass.Common.Systems
{
    public class WorldSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            
            int forestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Planting Trees"));

            if (forestIndex != -1)
            {
                tasks.Insert(forestIndex + 1, new ForestAmbient("Ironwood Trees", 100f));
            }
        }

        public class ForestAmbient : GenPass
        {
            public ForestAmbient(string name, float loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Ironwood Tree";

                int[] tileTypes = new int[] { ModContent.TileType<IronwoodSapling>() };

                // To not be annoying, we'll only spawn 15 Example Rubble near the spawn point.
                // This example uses the Try Until Success approach: https://github.com/tModLoader/tModLoader/wiki/World-Generation#try-until-success
                for (int k = 0; k < 10000; k++)
                {
                    bool success = false;
                    int attempts = 0;

                    while (!success)
                    {
                        attempts++;
                        if (attempts > 10000)
                        {
                            break;
                        }
                        int x = WorldGen.genRand.Next(200, Main.maxTilesX - 100);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, (int)Main.UnderworldLayer);
                        int tileType = WorldGen.genRand.Next(tileTypes);
                        

                        WorldGen.PlaceTile(x, y, tileType, mute: true);
                        
                        WorldGen.GrowTree(x, y);
                        success = Main.tile[x, y].TileType == tileType;
                    }
                }
            }
        }
    }
}
