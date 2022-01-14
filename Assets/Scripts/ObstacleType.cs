
[System.Flags] public enum ObstacleType
{
    floor = 1 << 1,
    stair = 1 << 2,
    wall = 1 << 3,
    npc = 1 << 4,
    terrain = 1 << 5,
    genericObstacle = 1 << 6,
}

//Useful tutorial on flags:
//https://www.youtube.com/watch?v=xUFCjIsqaZk