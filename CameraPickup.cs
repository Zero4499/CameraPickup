using GTA;
using GTA.Native;
using GTA.Math;
using System.Windows.Forms;

public class CameraPickup : Script
{
    private Prop cameraProp;
    private bool hasCamera = false;
    private bool camViewActive = false;
    private Camera cam;
    private float zoom = 50f;

    public CameraPickup()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
    }

    private void OnTick(object sender, System.EventArgs e)
    {
        if (camViewActive && cam != null)
        {
            Game.DisableAllControlsThisFrame(0);
            Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

            if (Game.IsControlPressed(0, Control.CursorScrollUp))
                zoom = Math.Max(10f, zoom - 1f);
            if (Game.IsControlPressed(0, Control.CursorScrollDown))
                zoom = Math.Min(100f, zoom + 1f);
            cam.FieldOfView = zoom;
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.C && !hasCamera)
        {
            Model model = new Model("prop_v_cam_01");
            model.Request(500);
            if (model.IsInCdImage && model.IsValid)
            {
                Vector3 pos = Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0f, 1f, 0f));
                cameraProp = World.CreateProp(model, pos, true, true);
                cameraProp.AttachTo(Game.Player.Character, Game.Player.Character.GetBoneIndex(Bone.RHand), new Vector3(0.1f, 0.0f, 0.0f), new Vector3(0f, 90f, 180f));
                hasCamera = true;
                UI.Notify("你拿起了摄像机！");
            }
        }

        if (e.KeyCode == Keys.M && hasCamera)
        {
            camViewActive = !camViewActive;

            if (camViewActive)
            {
                cam = World.CreateCamera(GameplayCamera.Position + GameplayCamera.ForwardVector * 0.5f, GameplayCamera.Rotation, zoom);
                World.RenderingCamera = cam;
                UI.Notify("摄影模式开启");
            }
            else
            {
                World.RenderingCamera = null;
                cam?.Delete();
                cam = null;
                UI.Notify("摄影模式关闭");
            }
        }

        if (e.KeyCode == Keys.Delete && hasCamera)
        {
            cameraProp?.Delete();
            hasCamera = false;
            camViewActive = false;
            World.RenderingCamera = null;
            UI.Notify("摄像机移除");
        }
    }
}