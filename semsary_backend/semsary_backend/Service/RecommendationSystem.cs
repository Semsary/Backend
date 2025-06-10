using System;
using System.Collections.Generic;
using Microsoft.ML.OnnxRuntime.Tensors;
using semsary_backend.Models;
using Microsoft.ML.OnnxRuntime;
using semsary_backend.Enums;
using Microsoft.AspNetCore.Http;
namespace semsary_backend.Service
{
    public class RecommendationSystem
    {
        private readonly InferenceSession item_scaler=new InferenceSession(Path.Combine("ML", "item_scaler"));
        private readonly InferenceSession user_scaler = new InferenceSession(Path.Combine("ML", "user_scaler"));
        private readonly InferenceSession recommender_model = new InferenceSession(Path.Combine("ML", "recommender_model"));
        public float[] Itemscaler(HouseInspection houseInspection,Governorate _gover)
        {
            float[]input=new float[56];

            input[0] = houseInspection.FloorNumber;
            input[1] = houseInspection.NumberOfAirConditionnar;
            input[2] = houseInspection.NumberOfBedRooms;
            input[3] = houseInspection.NumberOfPathRooms;
            input[4] = houseInspection.NumberOfBeds;
            input[5] = houseInspection.NumberOfBalacons;
            input[6] = houseInspection.NumberOfTables;
            input[7] = houseInspection.NumberOfChairs;
            input[8] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearHospital) ? 1.0f : 0.0f;
            input[9] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearGym) ? 1.0f : 0.0f;
            input[10] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearPlayGround) ? 1.0f : 0.0f;
            input[11] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearSchool) ? 1.0f : 0.0f;
            input[12] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearUniversity) ? 1.0f : 0.0f;
            input[13] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearSupermarket) ? 1.0f : 0.0f;
            input[14] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearRestaurant) ? 1.0f : 0.0f;
            input[15] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearBusStation) ? 1.0f : 0.0f;
            input[16] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveNearBank) ? 1.0f : 0.0f;
            input[17] = houseInspection.price;
            input[18] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveWiFi) ? 1.0f : 0.0f;
            input[19] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveTV) ? 1.0f : 0.0f;
            input[20] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.Havekitchen) ? 1.0f : 0.0f;
            input[21] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveElevator) ? 1.0f : 0.0f;
            input[22] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveWashingMachine) ? 1.0f : 0.0f;
            input[23] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveCooker) ? 1.0f : 0.0f;
            input[24] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveFridge) ? 1.0f : 0.0f;
            input[25] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveHeater) ? 1.0f : 0.0f;
            input[26] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveWiFi) ? 1.0f : 0.0f;
            input[27] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.HaveSalon) ? 1.0f : 0.0f;
            input[28] = houseInspection.HouseFeature.HasFlag(Enums.HouseFeature.DiningRoom) ? 1.0f : 0.0f;
            input[29] = _gover == Governorate.Cairo ? 1.0f : 0.0f;
            input[30] = _gover == Governorate.Alexandria ? 1.0f : 0.0f;
            input[31] = _gover == Governorate.Giza ? 1.0f : 0.0f;
            input[32] = _gover == Governorate.RedSea ? 1.0f : 0.0f;
            input[33] = _gover == Governorate.SouthSinai ? 1.0f : 0.0f;
            input[34] = _gover == Governorate.Aswan ? 1.0f : 0.0f;
            input[35] = _gover == Governorate.Damietta ? 1.0f : 0.0f;
            input[36] = _gover == Governorate.Dakahlia ? 1.0f : 0.0f;
            input[37] = _gover == Governorate.Gharbia ? 1.0f : 0.0f;
            input[38] = _gover == Governorate.Sharqia ? 1.0f : 0.0f;
            input[39] = _gover == Governorate.PortSaid ? 1.0f : 0.0f;
            input[40] = _gover == Governorate.Suez ? 1.0f : 0.0f;
            input[41] = _gover == Governorate.Ismailia ? 1.0f : 0.0f;
            input[42] = _gover == Governorate.Matrouh ? 1.0f : 0.0f;
            input[43] = _gover == Governorate.Assiut ? 1.0f : 0.0f;
            input[44] = _gover == Governorate.NewValley ? 1.0f : 0.0f;
            input[45] = _gover == Governorate.Beheira ? 1.0f : 0.0f;
            input[46] = _gover == Governorate.Qena ? 1.0f : 0.0f;
            input[47] = _gover == Governorate.Minya ? 1.0f : 0.0f;
            input[48] = _gover == Governorate.Luxor ? 1.0f : 0.0f;
            input[49] = _gover == Governorate.Qalyubia ? 1.0f : 0.0f;
            input[50] = _gover == Governorate.BeniSuef ? 1.0f : 0.0f;
            input[51] = _gover == Governorate.Fayoum ? 1.0f : 0.0f;
            input[52] = _gover == Governorate.KafrElSheikh ? 1.0f : 0.0f;
            input[53] = _gover == Governorate.Monufia ? 1.0f : 0.0f;
            input[54] = _gover == Governorate.NorthSinai ? 1.0f : 0.0f;
            input[55] = _gover == Governorate.Sohag ? 1.0f : 0.0f;
            var tensor = new DenseTensor<float>(input, new[] { 1, 56 });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };
            using (var results = item_scaler!.Run(inputs))
            {
                var output = results.FirstOrDefault()?.AsTensor<float>();
                if (output != null)
                {
                    return output.ToArray();
                }
            }
            return new float[56];

        }
        public float[] User_scaler(Tenant t)
        {
            float[] input = new float[38];
            input[0] = t.height;
            input[1] = t.gender == Gender.male ? 1.0f : 0.0f;
            input[2] = t.weight;
            input[3] = t.age;
            input[4] = t.IsSmoker?1.0f:0.0f;
            input[5] = t.FavouriteRentalType==RentalType2.Monthly?1.0f:0.0f;
            input[6] = t.NumberOfPeople;
            input[7]=t.NeedPublicService?1.0f:0.0f;
            input[8]=t.NeedPublicTransportation?1.0f:0.0f;
            input[9]=t.NeedNearUniversity?1.0f:0.0f;
            input[10]=t.NeedNearVitalPlaces?1.0f:0.0f;
            var _gover = t.Address._gover;
            input[11] = _gover == Governorate.Cairo ? 1.0f : 0.0f;
            input[12] = _gover == Governorate.Alexandria ? 1.0f : 0.0f;
            input[13] = _gover == Governorate.Giza ? 1.0f : 0.0f;
            input[14] = _gover == Governorate.RedSea ? 1.0f : 0.0f;
            input[15] = _gover == Governorate.SouthSinai ? 1.0f : 0.0f;
            input[16] = _gover == Governorate.Aswan ? 1.0f : 0.0f;
            input[17] = _gover == Governorate.Damietta ? 1.0f : 0.0f;
            input[18] = _gover == Governorate.Dakahlia ? 1.0f : 0.0f;
            input[19] = _gover == Governorate.Gharbia ? 1.0f : 0.0f;
            input[20] = _gover == Governorate.Sharqia ? 1.0f : 0.0f;
            input[21] = _gover == Governorate.PortSaid ? 1.0f : 0.0f;
            input[22] = _gover == Governorate.Suez ? 1.0f : 0.0f;
            input[23] = _gover == Governorate.Ismailia ? 1.0f : 0.0f;
            input[24] = _gover == Governorate.Matrouh ? 1.0f : 0.0f;
            input[25] = _gover == Governorate.Assiut ? 1.0f : 0.0f;
            input[26] = _gover == Governorate.NewValley ? 1.0f : 0.0f;
            input[27] = _gover == Governorate.Beheira ? 1.0f : 0.0f;
            input[28] = _gover == Governorate.Qena ? 1.0f : 0.0f;
            input[29] = _gover == Governorate.Minya ? 1.0f : 0.0f;
            input[30] = _gover == Governorate.Luxor ? 1.0f : 0.0f;
            input[31] = _gover == Governorate.Qalyubia ? 1.0f : 0.0f;
            input[32] = _gover == Governorate.BeniSuef ? 1.0f : 0.0f;
            input[33] = _gover == Governorate.Fayoum ? 1.0f : 0.0f;
            input[34] = _gover == Governorate.KafrElSheikh ? 1.0f : 0.0f;
            input[35] = _gover == Governorate.Monufia ? 1.0f : 0.0f;
            input[36] = _gover == Governorate.NorthSinai ? 1.0f : 0.0f;
            input[37] = _gover == Governorate.Sohag ? 1.0f : 0.0f;
            var tensor = new DenseTensor<float>(input, new[] { 1, 38 });
            var input1 = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input",tensor)
            };
            using (var results = item_scaler!.Run(input1))
            {
                var output = results.FirstOrDefault()?.AsTensor<float>();
                if (output != null)
                {
                    return output.ToArray();
                }
            }

            return new float[38];

        }
        public float recommend(Tenant t,HouseInspection item)
        {
            var userInput=User_scaler(t);
            var item_input = Itemscaler(item,item.House.governorate);
            var userTensor = new DenseTensor<float>(userInput, new[] { 1,38 });
            var itemTensor = new DenseTensor<float>(item_input, new[] { 1, 56 });
            var input = new List<NamedOnnxValue>();
            input.Add(NamedOnnxValue.CreateFromTensor("user_input",userTensor));
            input.Add(NamedOnnxValue.CreateFromTensor("apartment_input", itemTensor));
            using var results = recommender_model.Run(input);
            var output = results.First(r => r.Name == "dense_150").AsEnumerable<float>().First();

            return output;

        }
        

    }
}
