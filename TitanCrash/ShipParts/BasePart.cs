using Godot;
using System.Collections.Generic;

public class BasePart : Node2D
{
    [Export]
    public int[] PartLocation = new int[2]
    {
        0, // row
        0 // column
    };
    [Export]
    public Dictionary<string, float> Stats = new Dictionary<string, float>()
    {
        {"Engine", 0f},
        {"RCS", 0f},
        {"Structure", 50f},
        {"MaxStructure", 50f},
        {"Armor", 0f},
        {"MaxArmor", 0f},
        {"Sensors", 0f},
        {"PowerGeneration", 0f},
        {"PowerSupplied", 0f},
        {"PowerRequired", 0f},
        {"CrewSpace", 0f},
        {"CrewRequired", 0f},
        {"Crew", 0f},
        // TODO: Add heat to methods
        {"Heat", 0f},
        {"HeatSink", 0.5f},
        {"HeatThreshold", 5f}
    };
    public Dictionary<string, float> Surplus = new Dictionary<string, float>()
    {
        {"PowerSupplied", 0f},
        {"Crew", 0f},
        {"Heat", 0f}
    };
    public enum ModuleState
    {
        FULL,
        PARTIALFULL,
        HALF,
        DEFICIENT,
        NONE
    }
    public ModuleState CurrentCrewState = ModuleState.FULL;
    public ModuleState CurrentStructureState = ModuleState.FULL;
    public ModuleState CurrentPowerState = ModuleState.FULL;

    // Signals
    [Signal]
    public delegate void ModuleDestroyed(BasePart module);
    [Signal]
    public delegate void HeatSpreadAtLocation(int[] coordinates, float amount);
    [Signal]
    public delegate void HeatSinkAtLocation(int[] coordinates, float amount);

    // STANDARD
    public override void _Ready()
    {
        
    }

    // PLACEMENT
    public int GetRow()
    {
        return PartLocation[0];
    }
    public int GetColumn()
    {
        return PartLocation[1];
    }
    public void SetLocation(int[] newLocation)
    {
        if (newLocation.Length > 1)
        {
            PartLocation = newLocation;
        }
    }
    public void SetLocation(int row, int column)
    {
        PartLocation[0] = row;
        PartLocation[1] = column;
    }
    public void SetRow(int row)
    {
        PartLocation[0] = row;
    }
    public void SetColumn(int column)
    {
        PartLocation[1] = column;
    }
    public bool IsAtCoordinates(int[] location)
    {
        if (location.Length > 1)
        {
            return PartLocation[0] == location[0] && PartLocation[1] == location[1];
        }
        else
        {
            return false;
        }
    }
    public bool IsNeighbor(int[] location)
    {
        if (location.Length > 1)
        {
            return (location[0] == PartLocation[0] + 1 || location[0] == PartLocation[0] - 1) && (location[1] == PartLocation[1] + 1 || location[1] == PartLocation[1] - 1);
        }
        else
        {
            return false;
        }
    }
    public float GetEnginePower()
    {
        return Stats["Engine"];
    }
    public float GetRCSPower()
    {
        return Stats["RCS"];
    }
    // STRUCTURE
    public float GetStructure()
    {
        return Stats["Structure"];
    }
    public float GetMaxStructure()
    {
        return Stats["MaxStructure"];
    }
    // ARMOR
    public float GetArmor()
    {
        return Stats["Armor"];
    }
    public float GetMaxArmor()
    {
        return Stats["MaxArmor"];
    }
    // SENSORS
    public float GetSensorStrength()
    {
        return Stats["Sensors"];
    }
    // POWER
    public float GetPowerOutput()
    {
        return Stats["PowerGeneration"];
    }
    public float GetPowerSupplied()
    {
        return Stats["PowerSupplied"];
    }
    public float GetPowerRequired()
    {
        return Stats["PowerRequired"];
    }
    // CREW
    public int GetCrew()
    {
        return (int)Stats["Crew"];
    }
    public int GetRequiredCrew()
    {
        return (int)Stats["CrewRequired"];
    }
    public int GetCrewSpace()
    {
        return (int)Stats["CrewSpace"];
    }
    public int GetAvailableCrewVacancies()
    {
        return GetCrewSpace() - GetCrew();
    }
    public bool CrewFull()
    {
        return GetCrew() >= GetCrewSpace();
    }

    // HEAT
    public float GetHeat()
    {
        return Stats["Heat"];
    }
    public float GetHeatRadiation()
    {
        return (Stats["HeatSink"] * GetDamageReport()) + 0.1f;
    }
    public float GetMaxHeat()
    {
        return Stats["HeatThreshold"];
    }
    // This could end up recursively hitting the same modules over and over if not checked
    public void HeatUpdate()
    {
        ModifyStat("Heat", -GetHeatRadiation());
        if (Surplus["Heat"] > 0 || Surplus["Heat"] < 0)
        {
            float surplus = Surplus["Heat"];
            if (surplus > 0)
            {
                EmitSignal(nameof(HeatSpreadAtLocation), PartLocation, surplus);
            }
            else
            {
                EmitSignal(nameof(HeatSinkAtLocation), PartLocation, surplus);
                Surplus["Heat"] = 0f;
            }
            
        }
        if (IsDestroyed())
        {
            EmitSignal(nameof(ModuleDestroyed), this);
        }
    }
    public void TakeHeatDamage(float damage)
    {
        ModifyStat("Structure", damage*10f);
    }

    // EFFICIENCY CALCS
    public float GetCrewEffiency()
    {
        if (GetRequiredCrew() > 0)
        {
            if (GetCrew() > 0)
            {
                return GetCrew()/GetRequiredCrew();
            }
            else
            {
                return 0.0f;
            }
        }
        else
        {
            return 1.0f;
        }
    }
    public float GetDamageReport()
    {
        if (GetStructure() > 0f)
        {
            return GetStructure()/GetMaxStructure();
        }
        else
        {
            return 0f;
        }
    }
    public bool IsDestroyed()
    {
        return GetDamageReport() <= 0f;
    }
    public float GetPowerStatus()
    {
        if (GetPowerRequired() > 0f)
        {
            float currentPower = GetPowerOutput() + GetPowerSupplied();
            if (currentPower > 0f)
            {
                return currentPower/GetPowerRequired();
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            return 1.0f;
        }
    }
    public float GetArmorEfficiency()
    {
        if (GetMaxArmor() > 0f)
        {
            if (GetArmor() > 0f)
            {
                return GetArmor()/GetMaxArmor();
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            return 0f;
        }
    }
    public float GetTotalModuleEfficiency()
    {
        if (IsDestroyed() || GetPowerStatus() <= 0f || GetCrewEffiency() <= 0f)
        {
            return 0f;
        }
        else
        {
            return (GetPowerStatus() + GetStructure() + GetCrewEffiency())/3f;
        }

    }

    // STATE CHECKING
    public void CheckCrewState()
    {
        CurrentCrewState = CheckModuleState(GetCrewEffiency(), CurrentCrewState);
    }
    public void CheckStructureState()
    {
        CurrentStructureState = CheckModuleState(GetDamageReport(), CurrentStructureState);
    }
    public void CheckPowerState()
    {
        CurrentPowerState = CheckModuleState(GetPowerStatus(), CurrentPowerState);
    }
    public ModuleState CheckModuleState(float currentState, ModuleState stateCurrent)
    {
        ModuleState state = stateCurrent;
        if (currentState >= 1.0f)
        {
            state = ModuleState.FULL;
        }
        if (currentState < 1.0f && currentState >= 0.75f)
        {
            state = ModuleState.PARTIALFULL;
        }
        if (currentState >= 0.5f && currentState < 0.75f)
        {
            state = ModuleState.HALF;
        }
        if (currentState > 0f && currentState < 0.5f)
        {
            state = ModuleState.DEFICIENT;
        }
        if (currentState <= 0f)
        {
            state = ModuleState.NONE;
        }
        return state;
    }
    // REPORTING STAT (Used for total ship stat calculations, will not be used for armor or structure)
    public float GetStat(string stat)
    {
        if (Stats.ContainsKey(stat))
        {
            return Stats[stat] * GetTotalModuleEfficiency();
        }
        else
        {
            return 0f;
        }
    }

    public void SetStat(string stat, float value)
    {
        if (Stats.ContainsKey(stat))
        {
            Stats[stat] = value;
        }
    }
    public void ModifyStat(string stat, float value)
    {
        if (Stats.ContainsKey(stat))
        {
            Stats[stat] += value;
            float difference = 0f;
            if (Stats[stat] < 0f)
            {
                difference = Stats[stat];
                Stats[stat] = 0f;
            }
            if (stat == "PowerSupplied" && Stats[stat] > Stats["PowerRequired"])
            {
                Surplus[stat] = Stats[stat] - Stats["PowerRequired"];
                Stats[stat] = Stats["PowerRequired"];
            }
            if (stat == "Crew" && Stats[stat] > Stats["CrewSpace"])
            {
                Surplus[stat] = Stats[stat] - Stats["CrewSpace"];
                Stats[stat] = Stats["CrewSpace"];
            }
            if (stat == "Heat")
            {
                if (GetHeat() > GetMaxHeat())
                {
                    difference = GetHeat() - GetMaxHeat();
                    Stats["Heat"] = GetMaxHeat();
                }
                if (difference < 0 || difference > 0)
                {
                    Surplus["Heat"] += difference;
                }
            }

        }
    }
    
    // DAMAGE CALCULATIONS

    public void TakeDamage(float ballisticDamage, float armorPenetration=0f, float heatDamage=0f)
    {
        float armor = GetArmor() * GetArmorEfficiency();
        float modifiedBallistic = ballisticDamage;
        float armorDamage = 0f;
        float modifiedHeat = heatDamage;
        if (armor > 0f)
        {
            if (armorPenetration > 0)
            {
                armorDamage = armorPenetration/armor;
                ModifyStat("Armor", armorDamage);
                armor = GetArmor() * GetArmorEfficiency();
                if (armorDamage < 1.0f)
                {
                    modifiedBallistic *= armorDamage;
                }
            }
            if (armor > 0f)
            {
                modifiedBallistic /= armor * 20f;
                if (modifiedHeat > 0)
                {
                    modifiedHeat /= armor * 20f;
                }
                modifiedHeat /= armor * 20f;
            }
        }
        if (modifiedBallistic < 0.01)
        {
            modifiedBallistic = 0f;
        }
        if (modifiedHeat < 0.01)
        {
            modifiedHeat = 0f;
        }
        ModifyStat("Heat", modifiedHeat);
        ModifyStat("Structure", -modifiedBallistic);
        if (IsDestroyed())
        {
            EmitSignal(nameof(ModuleDestroyed));
        }
        
    }
}
