namespace semsary_backend.Enums
{
    [Flags]
    public enum HouseFeature
    {
        HaveNearHospital = 1 << 0,        // 1
        HaveNearGym = 1 << 1,             // 2
        HaveNearPlayGround = 1 << 2,      // 4
        HaveNearSchool = 1 << 3,          // 8
        HaveNearUniversity = 1 << 4,      // 16
        HaveNearSupermarket = 1 << 5,     // 32
        HaveNearRestaurant = 1 << 6,      // 64
        HaveNearBusStation = 1 << 7,      // 128
        HaveNearBank = 1 << 8,            // 256
        HaveWiFi = 1 << 9,                // 512
        HaveTV = 1 << 10,                 // 1024
        Havekitchen = 1 << 11,            // 2048
        HaveElevator = 1 << 12,           // 4096
        HaveWashingMachine = 1 << 13,     // 8192
        HaveCooker = 1 << 14,             // 16384
        HaveFridge = 1 << 15,             // 32768
        HaveHeater = 1 << 16,             // 65536
        HaveSalon = 1 << 17,              // 131072
        DiningRoom = 1 << 18              // 262144
    }
}
