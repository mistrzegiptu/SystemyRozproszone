import sys
import Ice
import SmartHome

def handle_light(proxy):
    print("\n--- Lights ---")
    print("1. Turn ON")
    print("2. Turn OFF")
    print("3. Get state")
    
    choice = input("Choose action: ")
    if choice == '1':
        proxy.TurnOn()
        print("-> Turned ON")
    elif choice == '2':
        proxy.TurnOff()
        print("-> Turned OFF")
    elif choice == '3':
        print(f"-> Light state: {'ON' if proxy.IsOn() else 'OFF'}")

def handle_rgb_light(proxy):
    print("\n--- RGB Light ---")
    print("1. Turn ON | 2. Turn OFF | 3. Get State")
    print("4. Set color (RGB)")
    print("5. Get current color")
    
    choice = input("Choose action: ")
    if choice in ['1', '2', '3']:
        if choice == '1': proxy.TurnOn(); print("-> Turned ON")
        elif choice == '2': proxy.TurnOff(); print("-> Turned OFF")
        elif choice == '3': print(f"-> Light state: {'ON' if proxy.IsOn() else 'OFF'}")
    elif choice == '4':
        r = int(input("Input R (0-255): "))
        g = int(input("Input G (0-255): "))
        b = int(input("Input B (0-255): "))
        color = SmartHome.Color(r, g, b)
        proxy.SetColor(color)
        print("-> Color has been set")
    elif choice == '5':
        color = proxy.GetColor()
        print(f"-> Current color: ({color.r}, {color.g}, {color.b})")

def handle_dimmer_light(proxy):
    print("\n--- Dimmer Light ---")
    print("1. Turn ON | 2. Turn OFF | 3. Get State")
    print("4. Set brightness level (0-100)")
    print("5. Get current brightness")
    
    choice = input("Choose action: ")
    if choice in ['1', '2', '3']:
        if choice == '1': proxy.TurnOn(); print("-> Turned ON")
        elif choice == '2': proxy.TurnOff(); print("-> Turned OFF")
        elif choice == '3': print(f"-> Light state: {'ON' if proxy.IsOn() else 'OFF'}")
    elif choice == '4':
        level = int(input("Input brightness (0-100): "))
        proxy.SetBrightness(level)
        print("-> Brightness has been set")
    elif choice == '5':
        print(f"-> Current brightness: {proxy.GetBrightness()}%")

def handle_thermostat(proxy):
    print("\n--- Thermostat ---")
    print("1. Set temperature")
    print("2. Get temperature")
    print("3. Set daily plan")
    print("4. Get daily plan")
    
    choice = input("Choose action: ")
    if choice == '1':
        temp = float(input("Input temperature: "))
        proxy.SetTemperature(temp)
        print("-> Temperature has been set")
    elif choice == '2':
        print(f"-> Current temperature: {proxy.GetTemperature()}°C")
    elif choice == '3':
        rules = []
        count = int(input("Input rules count: "))
        for i in range(count):
            print(f"Rule #{i+1}: ")
            t = input("  Time (eg. '08:00'): ")
            temp = float(input("  Target temperature: "))
            rules.append(SmartHome.ScheduleRule(t, temp))
        proxy.SetDailyPlan(rules)
        print("-> Daily plan has been set")
    elif choice == '4':
        plan = proxy.GetDailyPlan()
        if not plan:
            print("-> No daily plan set")
        for rule in plan:
            print(f"  [{rule.time}] -> {rule.targetTemp}°C")

def handle_roller_blind(proxy):
    print("\n--- Roller Blind ---")
    print("1. Set position (0-100%)")
    print("2. Get position")
    
    choice = input("Choose action: ")
    if choice == '1':
        pos = int(input("Input position in % (0 = open, 100 = closed): "))
        proxy.SetPosition(pos)
        print("-> Position has been set")
    elif choice == '2':
        print(f"-> Current position: {proxy.GetPosition()}%")

def handle_venetian_blind(proxy):
    print("\n--- Venetian Roller Blind ---")
    print("1. Set position (0-100%)")
    print("2. Get position")
    print("3. Set tilt angle")
    print("4. Get tilt angle")
    
    choice = input("Choose action: ")
    if choice == '1':
        pos = int(input("Input position in % (0 = open, 100 = closed): [SET TILT ANGLE TO 90° BEFORE]"))
        proxy.SetPosition(pos)
        print("-> Position has been set")
    elif choice == '2':
        print(f"-> Current position: {proxy.GetPosition()}%")
    elif choice == '3':
        angle = int(input("Input tilt angle (0° - 180°): "))
        proxy.SetTilt(angle)
        print("-> Tilt angle has been set")
    elif choice == '4':
        print(f"-> Current tilt angle: {proxy.GetTilt()}°")


def main():
    with Ice.initialize(sys.argv) as communicator:
        devices_config = [
            ("Living Room - Roller Blind", "RollerBlind_LivingRoom", 10000, SmartHome.RollerBlindPrx, handle_roller_blind),
            ("Living Room - RGB Light", "RGBLight_LivingRoom", 10000, SmartHome.RGBLightPrx, handle_rgb_light),
            ("Kitchen - Roller Blind", "RollerBlind_Kitchen", 10000, SmartHome.RollerBlindPrx, handle_roller_blind),
            ("Kitchen - Light", "Light_Kitchen", 10000, SmartHome.LightPrx, handle_light),
            ("Main Thermostat", "Thermostat_Main", 10000, SmartHome.ThermostatPrx, handle_thermostat),
            
            ("Bedroom - Light", "Light_Bedroom", 10001, SmartHome.LightPrx, handle_light),
            ("Bedroom - Dimmer Light", "DimmerLight_Bedroom", 10001, SmartHome.DimmerLightPrx, handle_dimmer_light),
            ("Bedroom - Roller Blind", "RollerBlind_Bedroom", 10001, SmartHome.RollerBlindPrx, handle_roller_blind),
            ("Bathroom - Venetian Blind Roller", "VenetianBlind_Bathroom", 10001, SmartHome.VenetianBlindPrx, handle_venetian_blind),
            ("Bathroom - Thermostat", "Thermostat_Bathroom", 10001, SmartHome.ThermostatPrx, handle_thermostat),
        ]

        proxies = []
        
        print("Connecting...")
        for label, identity, port, prx_class, handler in devices_config:
            base = communicator.stringToProxy(f"{identity}:tcp -h localhost -p {port}")
            proxy = prx_class.checkedCast(base)
            if not proxy:
                print(f"[Error] Can't connect with {label} on port {port}")
            else:
                proxies.append((label, proxy, handler))

        if not proxies:
            print("No devices found")
            return

        while True:
            print("\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")
            print("SMART HOME - CONTROL PANEL")
            print("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")
            for idx, (label, _, _) in enumerate(proxies):
                print(f"{idx + 1}. {label}")
            print("0. Exit")
            
            try:
                choice = int(input("\nChoose device: "))
                if choice == 0:
                    break
                
                if 1 <= choice <= len(proxies):
                    label, proxy, handler = proxies[choice - 1]
                    try:
                        handler(proxy)
                    except SmartHome.OutOfRangeException as e:
                        print(f"\n[ERROR] {e.reason}")
                    except SmartHome.HardwareErrorException as e:
                        print(f"\n[DEVICE ERROR] {e.details}")
                    except Ice.Exception as e:
                        print(f"\n[ICE ERROR] {e}")
                else:
                    print("Wrong choice")
            except ValueError:
                print("Input integer")

if __name__ == '__main__':
    main()