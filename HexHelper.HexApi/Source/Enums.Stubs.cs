using System;

/// <summary>
/// Stubs so that Enums.cs compiles.
/// Enums.cs is from Hex's code base.
/// </summary>
namespace Game.Shared.Mechanics
{
    [AttributeUsage( AttributeTargets.All )]
    public class ToolName : Attribute
    {
        private object mStuff;
        private object mStuff2;
        private bool mWhoknows;

        public ToolName( object aStuff = null, object aStuff2 = null, bool aWhoknows = false )
        {
            mStuff = aStuff;
            mStuff2 = aStuff2;
            mWhoknows = aWhoknows;
        }
    }

    public struct SessionCardId
    {
    };
}
