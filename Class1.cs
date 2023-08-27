using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;
using MenuAPI;
using Newtonsoft.Json.Linq;

namespace VehicleGarage;

public class VehicleGarage : Plugin
{
    public static List<VehicleHash> Vehicles = new List<VehicleHash>();
    public static JArray Vehconfig;
    public static JArray Garageconfig;
    private string _rank;
    private int _deptId;
    public static Vector3 MarkerLocation = new Vector3(442.25622558594f, -995.77874755859f, 21.336015701294f);

    public static Vector4 SpawnLocation =
        new Vector4(417.45257568359f, -1006.1844482422f, 21.336019515991f, 268.95791625977f);

    public static Vector3 DropOffLocation = new Vector3(415.25759887695f, -997.89276123047f, 21.336023330688f);
    public static List<JObject> Garages = new List<JObject>();
    public static bool Duty = false;
    public static Vehicle CurrentlyVisibleVehicleGarage = null;
    internal VehicleGarage()
    {
        Events.OnDutyStatusChange += EventsOnOnDutyStatusChange;
        Startup();
    }

    private async Task EventsOnOnDutyStatusChange(bool onduty)
    {
        Duty = onduty;
    }

    private async void Startup()
    {
        PlayerData data = Utilities.GetPlayerData();
        _rank = data.Rank;
        _deptId = data.DepartmentID;
        Vehconfig = (JArray)Utils.GetVehicleConfig()["police"];
        Garageconfig = Utils.GetGarageConfig();
        await ParseVehicles();
        await ParseGarages();
        Debug.WriteLine("Loaded VehicleGarage 1.0 by DevKilo");
    }

    
    

    private async Task ParseGarages()
    {
        foreach (var v in Garageconfig)
        {
            //Debug.WriteLine("another garage found in config");
            var garage = (JObject)v;
            
            JObject thisGarage = new JObject
            {
                ["isEnabled"] = (bool)garage["isEnabled"],
                ["name"] = (string)garage["name"],
                ["markerLocation"] = (JObject)garage["markerLocation"],
                ["spawnLocation"] = (JObject)garage["spawnLocation"],
                ["dropOffLocation"] = (JObject)garage["dropOffLocation"],
                ["isAvailableForEveryone"] = (bool)garage["isAvailableForEveryone"],
                ["useRanks"] = (bool)garage["useRanks"],
                ["availableForRanks"] = (JArray)garage["availableForRanks"],
                ["availableForDepartments"] = (JArray)garage["availableForDepartments"]
            };
            Garages.Add(thisGarage);
            //Debug.WriteLine("Adding garage");
        }

        foreach (JObject garage in Garages)
        {
            if (garage == null) return;
            //Debug.WriteLine("Another garage found in garages list");
            bool isEnabled = (bool)garage?["isEnabled"] || false;
            if (!isEnabled) continue;
            bool isAvailableForEveryone = (bool)garage?["isAvailableForEveryone"] || false;
            bool useRanks = (bool)garage?["useRanks"] || false;
            JArray availableForRanks = (JArray)garage?["availableForRanks"];
            JArray availableForDepartments = (JArray)garage?["availableForDepartments"];
            //Debug.WriteLine("After availableForDepartments");
            // Set perms

            if (!isAvailableForEveryone)
            {
                if (useRanks)
                {
                    if (!IsItemAMemberOfArray(_deptId.ToString(), availableForDepartments))
                    {
                        //Debug.WriteLine("User not member of department"); 
                        return;
                    }
                    if (!IsItemAMemberOfArray(_rank, availableForRanks)) return;
                    //Debug.WriteLine("running Draw markers");
                    VehicleMenu.DrawMarker(36, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                        (float)garage["markerLocation"]?["z"] - 0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, true,
                        false, 2, true,
                        null, null, false);
                    VehicleMenu.DrawMarker(1, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                        (float)garage["markerLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, false, false,
                        2, false,
                        null, null, false);
                    VehicleMenu.DrawMarker(1, (float)garage["dropOffLocation"]?["x"], (float)garage["dropOffLocation"]?["y"],
                        (float)garage["dropOffLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 255, 25, 25, 50, false, false,
                        2, false,
                        null, null, false);
                }
                else
                {
                    //Debug.WriteLine("This should be it");
                    if (!IsItemAMemberOfArray(_deptId.ToString(), availableForDepartments))
                    {
                        //Debug.WriteLine("User not member of department"); 
                        return;
                    }
                    ////Debug.WriteLine("Member is in department");
                    //Debug.WriteLine("Running draw marker");
                    VehicleMenu.DrawMarker(36, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                        (float)garage["markerLocation"]?["z"] - 0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, true,
                        false, 2, true,
                        null, null, false);
                    VehicleMenu.DrawMarker(1, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                        (float)garage["markerLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, false, false,
                        2, false,
                        null, null, false);
                    VehicleMenu.DrawMarker(1, (float)garage["dropOffLocation"]?["x"], (float)garage["dropOffLocation"]?["y"],
                        (float)garage["dropOffLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 255, 25, 25, 50, false, false,
                        2, false,
                        null, null, false);
                }
            }
            else
            {
                ////////Debug.WriteLine("Drawing marker");
                VehicleMenu.DrawMarker(36, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                    (float)garage["markerLocation"]?["z"] - 0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, true,
                    false, 2, true,
                    null, null, false);
                VehicleMenu.DrawMarker(1, (float)garage["markerLocation"]?["x"], (float)garage["markerLocation"]?["y"],
                    (float)garage["markerLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 77, 166, 255, 50, false, false,
                    2, false,
                    null, null, false);
                VehicleMenu.DrawMarker(1, (float)garage["dropOffLocation"]?["x"], (float)garage["dropOffLocation"]?["y"],
                    (float)garage["dropOffLocation"]?["z"] - 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 255, 25, 25, 50, false, false,
                    2, false,
                    null, null, false);    
            }
            
            
        }
    }

    private async Task ParseVehicles()
    {
        foreach (var vehicle in Vehconfig)
        {
            JObject veh = (JObject)vehicle;
            string vehName = (string)veh["name"];
            string spawnName = (string)veh["vehicle"];
            bool availableToEveryone = false;
            if (veh["isAvailableForEveryone"] != null)
                availableToEveryone = (bool)veh["isAvailableForEveryone"];

            if (!availableToEveryone)
            {
                JArray availableForRanks = null; 
                if (veh["availableForRanks"] != null)
                    availableForRanks = (JArray)veh["availableForRanks"];
                JArray availableForDepartments = null;
                if (veh["availableForDepartments"] != null)
                    availableForDepartments = (JArray)veh["availableForDepartments"];
                bool useRanks = false;
                if (veh["useRanks"] != null)
                    useRanks = (bool)veh["useRanks"];
                if (!useRanks)
                {
                    // Use departmentId only
                    bool result = IsItemAMemberOfArray(_deptId.ToString(), availableForDepartments);
                    if (result) AddVehicleToMenu(vehName, spawnName);
                }
                else
                {
                    // Use rank and departmentId
                    bool isrank = IsItemAMemberOfArray(_rank, availableForRanks);
                    bool isdept = IsItemAMemberOfArray(_deptId.ToString(), availableForDepartments);
                    if (isrank && isdept) AddVehicleToMenu(vehName,spawnName);
                }
            } 
            else AddVehicleToMenu(vehName,spawnName);
        }
    }

    private async void AddVehicleToMenu(string vehName, string spawnName)
    {
        MenuItem item = new(vehName, spawnName);
        VehicleMenu._menu.AddMenuItem(item);
    }

    private bool IsItemAMemberOfArray(string item, JArray array)
    {
        bool result = false;
        foreach (var v in array)
        {
            if (v.ToString() == item)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}

public class VehicleMenu : BaseScript
{
    public static Menu _menu;
    public static List<JObject> Markers = new List<JObject>();
    private Vehicle _lastVehicle;
    private int _dimensionId = 0;
    private Camera _oldCam;
    private MenuItem _currentMenuItem = null;
    private bool _canDelete = false;
    private List<Menu> _menusToCheckBeforeClose = new List<Menu>();
    public VehicleMenu()
    {
        Startup();
        _menu.OnItemSelect += MenuOnOnItemSelect;
        _menu.OnIndexChange += MenuOnOnIndexChange;
        _menu.OnMenuClose += MenuOnOnMenuClose;
        EventHandlers["KilosMultiverse:CurrentDimension"] += SetCurrentDimension;
    }

    private async void MenuOnOnMenuClose(Menu menu)
    {
        bool result = false;
        await Delay(100);
        foreach (Menu menu2 in _menusToCheckBeforeClose)
        {
            if (menu2.Visible)
                result = true;
        }
        ////Debug.WriteLine(result.ToString());
        if (!result)
        {

            if (VehicleGarage.CurrentlyVisibleVehicleGarage != null)
            {
                DeleteCurrentlySelectedVehicleInGarageInterior();
                Debug.WriteLine("Rawr");
            }
                
        }
            
    }

    private void SetCurrentDimension(int bucketId)
    {
        _dimensionId = bucketId;
    }

    private void MenuOnOnIndexChange(Menu menu, MenuItem olditem, MenuItem newitem, int oldindex, int newindex)
    {
        Debug.WriteLine("Rawrrrrr");
        DisplayCurrentlySelectedVehicleInGarageInterior();
    }

    
    private void Startup()
    {
        _menu = new Menu("Vehicle Garage", "Select a vehicle");
        MenuController.AddMenu(_menu);
        MenuController.MainMenu = _menu;
        _menusToCheckBeforeClose.Add(_menu);
        API.RegisterCommand("openVehicleSpawnMenu", new Action<int, List<object>, string>((source, args, rawCommand) =>
        {
            if (!VehicleGarage.Duty) return;
            ////////Debug.WriteLine("Got command");
            foreach (var garage in VehicleGarage.Garages)
            {
                Vector3 markerLocation = new Vector3((float)garage["markerLocation"]["x"],
                    (float)garage["markerLocation"]["y"], (float)garage["markerLocation"]["z"]);
                Vector3 dropOffLocation = new Vector3((float)garage["dropOffLocation"]["x"],
                    (float)garage["dropOffLocation"]["y"], (float)garage["dropOffLocation"]["z"]);
                if (Game.PlayerPed.Position.DistanceTo(markerLocation) < 1f)
                {
                    _menu.Visible = !_menu.Visible;
                    if (_menu.Visible == true)
                    {
                        _canDelete = false;
                        DisplayCurrentlySelectedVehicleInGarageInterior();
                    }
                }
                else if (Game.PlayerPed.Position.DistanceTo(dropOffLocation) < 5f)
                {
                    HandleVehicleDelete();
                }    
            }
            foreach (var marker in Markers)
            {
                //////Debug.WriteLine("marker");
                float f = (float)marker["posX"];
                Vector3 markerLocation = new Vector3((float)marker["posX"],
                    (float)marker["posY"],
                    (float)marker["posZ"]);
                //////Debug.WriteLine(markerLocation.ToString());
                
            }
            
        }), false);
        HandleMarkers();
        API.RegisterKeyMapping("openVehicleSpawnMenu","Vehicle Garage Interact Button","keyboard","e");
    }

    public static void DrawMarker(int type, float posX, float posY,float posZ,float dirX,float dirY,float dirZ,float rotX,float rotY, float rotZ, float scaleX, float scaleY, float scaleZ, int red, int green, int blue, int alpha, bool bobUpAndDown, bool faceCamera, int p19, bool rotate,string textureDict, string textureName, bool drawOnEnts)
    {
     //   if (!VehicleGarage.duty) return;
     JObject newMarker = new JObject()
     {
         ["type"] = type,
         ["posX"] = posX,
         ["posY"] = posY,
         ["posZ"] = posZ,
         ["dirX"] = dirX,
         ["dirY"] = dirY,
         ["dirZ"] = dirZ,
         ["rotX"] = rotX,
         ["rotY"] = rotY,
         ["rotZ"] = rotZ,
         ["scaleX"] = scaleX,
         ["scaleY"] = scaleY,
         ["scaleZ"] = scaleZ,
         ["red"] = red,
         ["green"] = green,
         ["blue"] = blue,
         ["alpha"] = alpha,
         ["bobUpAndDown"] = bobUpAndDown,
         ["faceCamera"] = faceCamera,
         ["p19"] = p19,
         ["rotate"] = rotate,
         ["textureDict"] = textureDict,
         ["textureName"] = textureName,
         ["drawOnEnts"] = drawOnEnts
     };
     Markers.Add(newMarker);
     //Debug.WriteLine("Added new marker");
        //API.DrawMarker(type,posX,posY,posZ,dirX,dirY,dirZ,rotX,rotY,rotZ,scaleX,scaleY,scaleZ,red,green,blue,alpha,bobUpAndDown,faceCamera,p19,rotate,textureDict,textureName,drawOnEnts);
        
    }

    private void HandleMarkers()
    {
        Tick += async () =>
        {
            if (!VehicleGarage.Duty) return;
            //Debug.WriteLine(Markers.Count.ToString());
            
            if (Markers.Count <= 0) return;
            //Debug.WriteLine("MARKER");
            foreach (var marker in Markers)
            {
                ////////Debug.WriteLine("Marker " + markers.IndexOf(marker).ToString());
                //Debug.WriteLine("MARKER create before");
                API.DrawMarker((int)marker["type"],(float)marker["posX"],(float)marker["posY"],(float)marker["posZ"],(float)marker["dirX"],(float)marker["dirY"],(float)marker["dirZ"],(float)marker["rotX"],(float)marker["rotY"],(float)marker["rotZ"],(float)marker["scaleX"],(float)marker["scaleY"],(float)marker["scaleZ"],(int)marker["red"],(int)marker["green"],(int)marker["blue"],(int)marker["alpha"],(bool)marker["bobUpAndDown"],(bool)marker["faceCamera"],(int)marker["p19"],(bool)marker["rotate"],(string)marker["textureDict"],(string)marker["textureName"],(bool)marker["drawOnEnts"]);
                //Debug.WriteLine("MARKER create");
            }
            
        };

    }

    public static string lastLiv = "Livery 0";
    private async Task DeleteCurrentlySelectedVehicleInGarageInterior([Optional] bool nodelete)
    {
        Debug.WriteLine("Nodelete: "+nodelete.ToString());
        API.DoScreenFadeIn(1000);  
        if (VehicleGarage.CurrentlyVisibleVehicleGarage == null) return;
        Vehicle veh = VehicleGarage.CurrentlyVisibleVehicleGarage;

        TriggerServerEvent("KilosMultiverse:DeleteDimension", _dimensionId);
        ////Debug.WriteLine("t");
        API.DoScreenFadeOut(1000);
        await Delay(1000);
        if (!nodelete)
        {
            lastLiv = API.GetLiveryName(veh.Handle, API.GetVehicleLivery(veh.Handle));
            Debug.WriteLine("Deleting veh");
            veh.Delete();
            VehicleGarage.CurrentlyVisibleVehicleGarage = null;    
        }
        else
        {
            ////Debug.WriteLine("Sending vehicle to 0");
            TriggerServerEvent("KilosMultiverse:RemoveEntityFromDimension",veh.NetworkId);
        }
        World.RenderingCamera.Delete();
        API.RenderScriptCams(false, false, 0, false, false);
        API.DoScreenFadeIn(1000);
    }
    
    private async Task DisplayCurrentlySelectedVehicleInGarageInterior()
    {
        if (VehicleGarage.CurrentlyVisibleVehicleGarage != null)
        {
            DeleteCurrentlySelectedVehicleInGarageInterior();
        }
        
        Vector4 vehicleGarageSpawnPos =
            new Vector4(405.0553894043f, -969.68841552734f, -99.004188537598f, 52.327861785889f);
        Vector4 camPos = new Vector4(404.65289306641f, -962.37182617188f, -98.504188537598f, 182.31213378906f);
        string spawnCode = VehicleMenu._menu.GetCurrentMenuItem().Description;
        int hash = API.GetHashKey(spawnCode);
        TriggerServerEvent("KilosMultiverse:CreateNewDimension");
        API.DoScreenFadeIn(1000);
        API.DoScreenFadeOut(1000);
        await Delay(1000);
        if (_dimensionId == -1)
        {
            ////Debug.WriteLine("Dimension is -1!");
            return;
        }
        Debug.WriteLine("Before spawn");
        Vehicle veh = await World.CreateVehicle(new(hash),
            new(vehicleGarageSpawnPos.X, vehicleGarageSpawnPos.Y, vehicleGarageSpawnPos.Z - 0.97f), vehicleGarageSpawnPos.W);
        veh.IsPositionFrozen = true;
        veh.IsCollisionEnabled = false;
        veh.CanBeVisiblyDamaged = false;
        TriggerServerEvent("KilosMultiverse:AddEntityToDimension", veh.NetworkId,_dimensionId);
        VehicleGarage.CurrentlyVisibleVehicleGarage = veh;
        //Game.PlayerPed.Position = veh.Position;
        Camera cam = World.CreateCamera((Vector3)camPos, new(camPos.W, 180f, 0f), 50f);
        API.SetVehicleModKit(veh.Handle, 0);
        API.SetVehicleLivery(veh.Handle, 0);
        //await Delay(2000);
        _oldCam = World.RenderingCamera;
        World.RenderingCamera = cam;
        API.DoScreenFadeIn(1000);
    }

    private async void HandleVehicleDelete()
    {
        if (Game.PlayerPed.IsInVehicle())
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            Game.PlayerPed.Task.LeaveVehicle();
            await Delay(1500);
            veh.Delete();
        }
    }

    private async void MenuOnOnItemSelect(Menu menu, MenuItem menuitem, int itemindex)
    {
        ////Debug.WriteLine("Beginning");
        if (menu == _menu)
        {
            // Prerequisites
            if (_lastVehicle != null && _lastVehicle.Exists()) _lastVehicle.Delete();
            _currentMenuItem = menuitem;
            _canDelete = false;
            Vehicle veh = VehicleGarage.CurrentlyVisibleVehicleGarage;
            // Vehicle Menu
            Menu vehMenu = new Menu("Police Garage", "Customization");
            MenuController.AddMenu(vehMenu);
            MenuController.BindMenuItem(VehicleMenu._menu, vehMenu, menuitem);
            _menusToCheckBeforeClose.Add(vehMenu);
            // Save & Continue
            MenuItem spawnButton = new MenuItem("Save & Continue");
            spawnButton.ItemData = veh.Handle;
            vehMenu.AddMenuItem(spawnButton);
            // Extras Menu
            Menu extrasMenu = new Menu("Police Garage", "Customization: Extras");
            MenuItem extraMenuButton = new MenuItem("Extras");
            extraMenuButton.ItemData = veh.Handle;
            vehMenu.AddMenuItem(extraMenuButton);
            MenuController.AddMenu(extrasMenu);
            MenuController.BindMenuItem(vehMenu, extrasMenu, extraMenuButton);
            _menusToCheckBeforeClose.Add(extrasMenu);
            // Livery Menu
            Menu liveryMenu = new Menu("Police Garage", "Customization: Livery");
            MenuItem liveryMenuButton = new MenuItem("Livery");
            liveryMenuButton.ItemData = veh.Handle;
            vehMenu.AddMenuItem(liveryMenuButton);
            MenuController.AddMenu(liveryMenu);
            MenuController.BindMenuItem(vehMenu, liveryMenu, liveryMenuButton);
            _menusToCheckBeforeClose.Add(liveryMenu);
            
            List<int> liveries = new List<int>();
            // Add Livery To Menu
            int maxLiveries = API.GetVehicleLiveryCount(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle);
            for (int i = 0; i <= maxLiveries; i++)
            {
                string liveryName = API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, i);
                    if (liveryName != "")
                        liveries.Add(i);
            }
            string getInitialName =
                API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, liveries[0]);
            if (getInitialName == null)
                getInitialName = "Livery 0";
            
            MenuDynamicListItem liverySelection = new MenuDynamicListItem("Livery", getInitialName,
                (item, left) =>
                {
                    ////Debug.WriteLine("ddd");
                        int index = 0;
                        string name;
                        foreach (int liveryIndex in liveries)
                        {
                            if (API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle,
                                    liveryIndex) == item.CurrentItem)
                            {
                                index = liveryIndex;
                                break;
                            }
                        }
                        if (index > maxLiveries)
                            index = maxLiveries - 1;
                        if (index == -1) index = 0;

                        API.SetVehicleModKit(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, 0);
                        if (left)
                        {
                            
                            if (index == 0)
                            {
                                API.SetVehicleLivery(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, index);
                                name = API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle,
                                    index);
                                if (name == null)
                                {
                                    name = "Livery "+index;
                                }
                            }
                            else
                            {
                                index = index - 1;
                                API.SetVehicleLivery(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, index);
                                name = API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle,
                                    index);
                                if (name == null)
                                {
                                    name = "Livery "+(index);
                                }

                                
                            }
                        }
                        else
                        {
                            
                            index = index + 1;
                            
                                
                                API.SetVehicleLivery(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle, index);
                                name = API.GetLiveryName(VehicleGarage.CurrentlyVisibleVehicleGarage.Handle,
                                    index);
                                if (name == null)
                                {
                                    name = "Livery "+(index);
                                }
                        }

                        name = "Livery " + index;
                        ////Debug.WriteLine(index.ToString());
                        return name;

                    },"Select a livery");
            liveryMenu.AddMenuItem(liverySelection);
            ////Debug.WriteLine("Before vehicle");
            // Select Vehicle
            vehMenu.OnItemSelect += async (menu1, item, index) =>
            {
                ////Debug.WriteLine("itemste");
                if (item == spawnButton)
                {
                    Debug.WriteLine("Spawn button pressed");
                    // Spawn vehicle here
                    //DeleteCurrentlySelectedVehicleInGarageInterior();
                    API.DoScreenFadeOut(1000);
                    await Delay(1000);
                    List<MenuCheckboxItem> extras = new List<MenuCheckboxItem>();
                    // Cache Extras
                    foreach (MenuItem item2 in extrasMenu.GetMenuItems())
                    {
                        if (item2 != spawnButton)
                        {
                            extras.Add((MenuCheckboxItem)item2);
                        }
                    }
                    int hash = veh.Model.Hash;
                    Debug.WriteLine("Before newveh spawns");
                    Vehicle newveh = await World.CreateVehicle(new(hash), (Vector3)VehicleGarage.SpawnLocation,
                        VehicleGarage.SpawnLocation.W);
                    Debug.WriteLine("After newveh spawns");
                    Debug.WriteLine("check2");
                    _lastVehicle = Game.PlayerPed.CurrentVehicle;
                    // Set extras default state
                    foreach (var extra in extras)
                    {
                        string t = extra.Text.Replace("Extra ", "").Trim();
                        int i = Int32.Parse(t);
                        ////Debug.WriteLine("Before set extra: "+i);
                        ////Debug.WriteLine(extra.Checked.ToString());
                        API.SetVehicleExtra(newveh.Handle, i, !extra.Checked);
                        Debug.WriteLine("after set extra: "+i);
                    }
                    // Set livery
                    //Debug.WriteLine(liverySelection.CurrentItem);
                    //int liv = Int32.Parse(liverySelection.CurrentItem.Replace("Livery ", "").Trim());
                    ////Debug.WriteLine("Before livery");
                    //Debug.WriteLine(liv.ToString());

                    ////Debug.WriteLine("Before set livery: "+liv)
                    //API.SetVehicleLivery(newveh.Handle,liv);
                    Debug.WriteLine("check3");
                    if (VehicleGarage.CurrentlyVisibleVehicleGarage == newveh)
                        Debug.WriteLine("Newveh is oldveh");
                    DeleteCurrentlySelectedVehicleInGarageInterior();
                    //SetPlayerIntoVehicleAfterSpawn(newveh);
                    await Delay(1000);
                    API.DoScreenFadeIn(1000);
                    await Delay(2000);
                    Debug.WriteLine("Before menus close");
                    foreach (var menux in _menusToCheckBeforeClose)
                    {
                        menux.CloseMenu();
                        menux.Visible = false;
                        Debug.WriteLine("Menu closing");
                    }
                    Debug.WriteLine("After menus close");
                    //
                }
            };
            ////Debug.WriteLine("before extras");
            // Toggle Extras
            extrasMenu.OnCheckboxChange += (menux, menuItem, index, state) =>
            {
                string t = menuItem.Text.Replace("Extra ", "").Trim();
                Debug.WriteLine(t);
                
                int i = Int32.Parse(t);
                ////Debug.WriteLine("Parse successful: "+i.ToString());
                // Vehicle handle
                API.SetVehicleExtra(veh.Handle, i, !state);
            };
            // Add extras to menu
            int maxExtras = 50;
            for (int i = 0; i < maxExtras; i++)
            {
                if (API.DoesExtraExist(veh.Handle, i))
                {
                    MenuCheckboxItem extra = new MenuCheckboxItem("Extra " + i);
                    extra.Checked = API.IsVehicleExtraTurnedOn(veh.Handle, i) || false;
                    extrasMenu.AddMenuItem(extra);
                    Debug.WriteLine("Thing");
                }
            }
        }
        
    }

    private async Task WaitUntilVehicleSpawns(Vehicle veh, Vector3 spawnPos)
    {
        while (true)
        {
            if (veh != null && veh.Exists())
            {
                await WaitUntilEntityIsAtPosition(veh,spawnPos);
                break;
            }
                
            await Delay(300);
        }
        Debug.WriteLine("Vehicle should be spawned");
    }

    private async Task WaitUntilEntityIsAtPosition(Entity ent, Vector3 pos)
    {
        while (true)
        {
            if (ent == null && !ent.Exists())
                break;
            if (ent.Position == pos)
                break;
            await Delay(300);
        }
    }

    private async Task SetPlayerIntoVehicleAfterSpawn(Vehicle veh)
    {
        await WaitUntilVehicleSpawns(veh,(Vector3)VehicleGarage.SpawnLocation);
        Game.PlayerPed.SetIntoVehicle(veh,VehicleSeat.Driver);
    }
    
}
