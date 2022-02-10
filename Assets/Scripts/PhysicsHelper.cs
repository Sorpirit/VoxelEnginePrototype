using Data;

public static class PhysicsHelper
{
    public static bool IsSolid(this BlockType type)
    {
        return type != BlockType.Air;
    }
    
    public static bool IsEmpty(this BlockType type)
    {
        return type == BlockType.Air;
    }
}