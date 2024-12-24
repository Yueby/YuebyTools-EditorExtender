using System.Linq;
using UnityEditor;
using Yueby.EditorWindowExtends.AnimatorWindowExtends.Core;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;

namespace Yueby.EditorWindowExtends.AnimatorWindowExtends
{
    public class GraphGUIExtender : EditorExtender<GraphGUIExtender, GraphGUIDrawer>
    {
        static GraphGUIExtender()
        {
            Instance = new GraphGUIExtender();
        }

        public override string Name => "Graph View";

        public static GraphGUIExtender Instance { get; }

        public static void OnDrawStateNode(GraphGUI graphGUI, StateNode stateNode)
        {
            foreach (var drawer in Instance.ExtenderDrawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnDrawGraphGUI(graphGUI, stateNode);
            }

            // if (
            //     EditorWindow.mouseOverWindow != null
            //     && EditorWindow.mouseOverWindow.GetType().Name == "AnimatorControllerTool"
            // )
            // {
            //     EditorWindow.mouseOverWindow.Repaint();
            // }
        }
    }
}
