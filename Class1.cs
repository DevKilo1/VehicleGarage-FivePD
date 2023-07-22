using System;
using System.Collections.Generic;
using System.Drawing;
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
    public static List<VehicleHash> vehicles = new List<VehicleHash>();
    public static JArray vehconfig;
    public static JArray garageconfig;
    private string rank;
    private int deptId;
    public static Vector3 markerLocation = new Vector3(442.25622558594f, -995.77874755859f, 21.336015701294f);

    public static Vector4 spawnLocation =
        new Vector4(417.45257568359f, -1006.1844482422f, 21.336019515991f, 268.95791625977f);

    public static Vector3 dropOffLocation = new Vector3(415.25759887695f, -997.89276123047f, 21.336023330688f);
    public static List<JObject> garages = new List<JObject>();
    public static bool duty = false;
    internal VehicleGarage()
    {
        Events.OnDutyStatusChange += EventsOnOnDutyStatusChange;
        Startup();
    }

    private async Task EventsOnOnDutyStatusChange(bool onduty)
    {
        duty = onduty;
    }

    private async void Startup()
    {
        PlayerData data = Utilities.GetPlayerData();
        rank = data.Rank;
        deptId = data.DepartmentID;
        vehconfig = (JArray)Utils.GetVehicleConfig()["police"];
        garageconfig = Utils.GetGarageConfig();
        await ParseVehicles();
        await ParseGarages();
        //Debug.WriteLine("Loaded VehicleGarage 1.0 by DevKilo");
    }

    private async Task ParseGarages()
    {
        foreach (var v in garageconfig)
        {
            ////Debug.WriteLine("another garage found in config");
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
            garages.Add(thisGarage);
        }

        foreach (JObject garage in garages)
        {
            if (garage == null) return;
            ////Debug.WriteLine("Another garage found in garages list");
            bool isEnabled = (bool)garage?["isEnabled"] || false;
            if (!isEnabled) continue;
            bool isAvailableForEveryone = (bool)garage?["isAvailableForEveryone"] || false;
            bool useRanks = (bool)garage?["useRanks"] || false;
            JArray availableForRanks = (JArray)garage?["availableForRanks"];
            JArray availableForDepartments = (JArray)garage?["availableForDepartments"];
            
            // Set perms

            if (!isAvailableForEveryone)
            {
                if (useRanks)
                {
                    if (!IsItemAMemberOfArray(deptId.ToString(), availableForDepartments))
                    {
                      //  //Debug.WriteLine("User not member of department"); 
                        return;
                    }
                    if (!IsItemAMemberOfArray(rank, availableForRanks)) return;
                    ////Debug.WriteLine("Drawing marker");
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
                    if (!IsItemAMemberOfArray(deptId.ToString(), availableForDepartments))
                    {
                        ////Debug.WriteLine("User not member of department"); 
                        return;
                    }
                    if (!IsItemAMemberOfArray(rank, availableForRanks)) return;
                    ////Debug.WriteLine("Drawing marker");
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
                ////Debug.WriteLine("Drawing marker");
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
        foreach (var vehicle in vehconfig)
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
                    bool result = IsItemAMemberOfArray(deptId.ToString(), availableForDepartments);
                    if (result) AddVehicleToMenu(vehName, spawnName);
                }
                else
                {
                    // Use rank and departmentId
                    bool isrank = IsItemAMemberOfArray(rank, availableForRanks);
                    bool isdept = IsItemAMemberOfArray(deptId.ToString(), availableForDepartments);
                    if (isrank && isdept) AddVehicleToMenu(vehName,spawnName);
                }
            } 
            else AddVehicleToMenu(vehName,spawnName);
        }
    }

    private void AddVehicleToMenu(string vehName, string spawnName)
    {
        VehicleMenu._menu.AddMenuItem(new(vehName,spawnName));
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
    public static List<JObject> markers = new List<JObject>();
    private Vehicle lastVehicle;
    public VehicleMenu()
    {
        Startup();
        _menu.OnItemSelect += MenuOnOnItemSelect;
    }

    private void Startup()
    {
        _menu = new Menu("Vehicle Garage", "Select a vehicle");
        MenuController.AddMenu(_menu);
        MenuController.MainMenu = _menu;
        API.RegisterCommand("openVehicleSpawnMenu", new Action<int, List<object>, string>((source, args, rawCommand) =>
        {
            if (!VehicleGarage.duty) return;
            ////Debug.WriteLine("Got command");
            foreach (var garage in VehicleGarage.garages)
            {
                Vector3 markerLocation = new Vector3((float)garage["markerLocation"]["x"],
                    (float)garage["markerLocation"]["y"], (float)garage["markerLocation"]["z"]);
                Vector3 dropOffLocation = new Vector3((float)garage["dropOffLocation"]["x"],
                    (float)garage["dropOffLocation"]["y"], (float)garage["dropOffLocation"]["z"]);
                if (Game.PlayerPed.Position.DistanceTo(markerLocation) < 1f)
                {
                    _menu.Visible = !_menu.Visible;
                }
                else if (Game.PlayerPed.Position.DistanceTo(dropOffLocation) < 5f)
                {
                    handleVehicleDelete();
                }    
            }
            foreach (var marker in markers)
            {
                //Debug.WriteLine("marker");
                float f = (float)marker["posX"];
                Vector3 markerLocation = new Vector3((float)marker["posX"],
                    (float)marker["posY"],
                    (float)marker["posZ"]);
                //Debug.WriteLine(markerLocation.ToString());
                
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
     markers.Add(newMarker);
     ////Debug.WriteLine("Added new marker");
        //API.DrawMarker(type,posX,posY,posZ,dirX,dirY,dirZ,rotX,rotY,rotZ,scaleX,scaleY,scaleZ,red,green,blue,alpha,bobUpAndDown,faceCamera,p19,rotate,textureDict,textureName,drawOnEnts);
        
    }

    private void HandleMarkers()
    {
        Tick += async () =>
        {
            if (!VehicleGarage.duty) return;
            ////Debug.WriteLine(markers.Count.ToString());
            if (markers.Count <= 0) return;
            foreach (var marker in markers)
            {
                ////Debug.WriteLine("Marker " + markers.IndexOf(marker).ToString());
                API.DrawMarker((int)marker["type"],(float)marker["posX"],(float)marker["posY"],(float)marker["posZ"],(float)marker["dirX"],(float)marker["dirY"],(float)marker["dirZ"],(float)marker["rotX"],(float)marker["rotY"],(float)marker["rotZ"],(float)marker["scaleX"],(float)marker["scaleY"],(float)marker["scaleZ"],(int)marker["red"],(int)marker["green"],(int)marker["blue"],(int)marker["alpha"],(bool)marker["bobUpAndDown"],(bool)marker["faceCamera"],(int)marker["p19"],(bool)marker["rotate"],(string)marker["textureDict"],(string)marker["textureName"],(bool)marker["drawOnEnts"]);    
            }
            
            
        };

    }

    private async void handleVehicleDelete()
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
        if (menu == _menu)
        {
            if (lastVehicle != null && lastVehicle.Exists()) lastVehicle.Delete();
            int hash = API.GetHashKey(menuitem.Description);
            Vehicle vehicle = await World.CreateVehicle(new(hash), (Vector3)VehicleGarage.spawnLocation, VehicleGarage.spawnLocation.W);
            if (vehicle == null || !vehicle.Exists()) return;
            Game.PlayerPed.SetIntoVehicle(vehicle,VehicleSeat.Driver);
            lastVehicle = Game.PlayerPed.CurrentVehicle;
            ////Debug.WriteLine("Set player into spawned vehicle");
        }
    }
}