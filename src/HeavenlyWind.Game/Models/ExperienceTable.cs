﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public static class ExperienceTable
    {
        public static IDTable<Experience> Ship { get; private set; }
        public static IDTable<Experience> Admiral { get; }

        static ExperienceTable()
        {
            if (!DataStore.TryGet("ship_experience", out byte[] content))
                ShipExperienceFallback();
            else
            {
                var reader = new JsonTextReader(new StreamReader(new MemoryStream(content)));
                var data = JArray.Load(reader).ToObject<int[]>();

                var table = new SortedList<int, Experience>(data.Length);

                var acc = 0;

                for (var i = 0; i < data.Length; i++)
                {
                    if (i > 0)
                        acc += table[i].Next;

                    var next = 0;
                    if (i < data.Length - 1)
                        next = data[i + 1] - data[i];

                    table[i + 1] = new Experience(i + 1, acc, next);
                }

                Ship = new IDTable<Experience>(table);
            }

            Admiral = new IDTable<Experience>(new SortedList<int, Experience>(120)
            {
                { 1, new Experience(1, 0, 100) },
                { 2, new Experience(2, 100, 200) },
                { 3, new Experience(3, 300, 300) },
                { 4, new Experience(4, 600, 400) },
                { 5, new Experience(5, 1000, 500) },
                { 6, new Experience(6, 1500, 600) },
                { 7, new Experience(7, 2100, 700) },
                { 8, new Experience(8, 2800, 800) },
                { 9, new Experience(9, 3600, 900) },
                { 10, new Experience(10, 4500, 1000) },
                { 11, new Experience(11, 5500, 1100) },
                { 12, new Experience(12, 6600, 1200) },
                { 13, new Experience(13, 7800, 1300) },
                { 14, new Experience(14, 9100, 1400) },
                { 15, new Experience(15, 10500, 1500) },
                { 16, new Experience(16, 12000, 1600) },
                { 17, new Experience(17, 13600, 1700) },
                { 18, new Experience(18, 15300, 1800) },
                { 19, new Experience(19, 17100, 1900) },
                { 20, new Experience(20, 19000, 2000) },
                { 21, new Experience(21, 21000, 2100) },
                { 22, new Experience(22, 23100, 2200) },
                { 23, new Experience(23, 25300, 2300) },
                { 24, new Experience(24, 27600, 2400) },
                { 25, new Experience(25, 30000, 2500) },
                { 26, new Experience(26, 32500, 2600) },
                { 27, new Experience(27, 35100, 2700) },
                { 28, new Experience(28, 37800, 2800) },
                { 29, new Experience(29, 40600, 2900) },
                { 30, new Experience(30, 43500, 3000) },
                { 31, new Experience(31, 46500, 3100) },
                { 32, new Experience(32, 49600, 3200) },
                { 33, new Experience(33, 52800, 3300) },
                { 34, new Experience(34, 56100, 3400) },
                { 35, new Experience(35, 59500, 3500) },
                { 36, new Experience(36, 63000, 3600) },
                { 37, new Experience(37, 66600, 3700) },
                { 38, new Experience(38, 70300, 3800) },
                { 39, new Experience(39, 74100, 3900) },
                { 40, new Experience(40, 78000, 4000) },
                { 41, new Experience(41, 82000, 4100) },
                { 42, new Experience(42, 86100, 4200) },
                { 43, new Experience(43, 90300, 4300) },
                { 44, new Experience(44, 94600, 4400) },
                { 45, new Experience(45, 99000, 4500) },
                { 46, new Experience(46, 103500, 4600) },
                { 47, new Experience(47, 108100, 4700) },
                { 48, new Experience(48, 112800, 4800) },
                { 49, new Experience(49, 117600, 4900) },
                { 50, new Experience(50, 122500, 5000) },
                { 51, new Experience(51, 127500, 5200) },
                { 52, new Experience(52, 132700, 5400) },
                { 53, new Experience(53, 138100, 5600) },
                { 54, new Experience(54, 143700, 5800) },
                { 55, new Experience(55, 149500, 6000) },
                { 56, new Experience(56, 155500, 6200) },
                { 57, new Experience(57, 161700, 6400) },
                { 58, new Experience(58, 168100, 6600) },
                { 59, new Experience(59, 174700, 6800) },
                { 60, new Experience(60, 181500, 7000) },
                { 61, new Experience(61, 188500, 7300) },
                { 62, new Experience(62, 195800, 7600) },
                { 63, new Experience(63, 203400, 7900) },
                { 64, new Experience(64, 211300, 8200) },
                { 65, new Experience(65, 219500, 8500) },
                { 66, new Experience(66, 228000, 8800) },
                { 67, new Experience(67, 236800, 9100) },
                { 68, new Experience(68, 245900, 9400) },
                { 69, new Experience(69, 255300, 9700) },
                { 70, new Experience(70, 265000, 10000) },
                { 71, new Experience(71, 275000, 10400) },
                { 72, new Experience(72, 285400, 10800) },
                { 73, new Experience(73, 296200, 11200) },
                { 74, new Experience(74, 307400, 11600) },
                { 75, new Experience(75, 319000, 12000) },
                { 76, new Experience(76, 331000, 12400) },
                { 77, new Experience(77, 343400, 12800) },
                { 78, new Experience(78, 356200, 13200) },
                { 79, new Experience(79, 369400, 13600) },
                { 80, new Experience(80, 383000, 14000) },
                { 81, new Experience(81, 397000, 14500) },
                { 82, new Experience(82, 411500, 15000) },
                { 83, new Experience(83, 426500, 15500) },
                { 84, new Experience(84, 442000, 16000) },
                { 85, new Experience(85, 458000, 16500) },
                { 86, new Experience(86, 474500, 17000) },
                { 87, new Experience(87, 491500, 17500) },
                { 88, new Experience(88, 509000, 18000) },
                { 89, new Experience(89, 527000, 18500) },
                { 90, new Experience(90, 545500, 19000) },
                { 91, new Experience(91, 564500, 20000) },
                { 92, new Experience(92, 584500, 22000) },
                { 93, new Experience(93, 606500, 25000) },
                { 94, new Experience(94, 631500, 30000) },
                { 95, new Experience(95, 661500, 40000) },
                { 96, new Experience(96, 701500, 60000) },
                { 97, new Experience(97, 761500, 90000) },
                { 98, new Experience(98, 851500, 148500) },
                { 99, new Experience(99, 1000000, 300000) },
                { 100, new Experience(100, 1300000, 300000) },
                { 101, new Experience(101, 1600000, 300000) },
                { 102, new Experience(102, 1900000, 300000) },
                { 103, new Experience(103, 2200000, 400000) },
                { 104, new Experience(104, 2600000, 400000) },
                { 105, new Experience(105, 3000000, 500000) },
                { 106, new Experience(106, 3500000, 500000) },
                { 107, new Experience(107, 4000000, 600000) },
                { 108, new Experience(108, 4600000, 600000) },
                { 109, new Experience(109, 5200000, 700000) },
                { 110, new Experience(110, 5900000, 700000) },
                { 111, new Experience(111, 6600000, 800000) },
                { 112, new Experience(112, 7400000, 800000) },
                { 113, new Experience(113, 8200000, 900000) },
                { 114, new Experience(114, 9100000, 900000) },
                { 115, new Experience(115, 10000000, 1000000) },
                { 116, new Experience(116, 11000000, 1000000) },
                { 117, new Experience(117, 12000000, 1000000) },
                { 118, new Experience(118, 13000000, 1000000) },
                { 119, new Experience(119, 14000000, 1000000) },
                { 120, new Experience(120, 15000000, 0) },
            });
        }

        static void ShipExperienceFallback()
        {
            Ship = new IDTable<Experience>(new SortedList<int, Experience>(155)
            {
                { 1, new Experience(1, 0, 100) },
                { 2, new Experience(2, 100, 200) },
                { 3, new Experience(3, 300, 300) },
                { 4, new Experience(4, 600, 400) },
                { 5, new Experience(5, 1000, 500) },
                { 6, new Experience(6, 1500, 600) },
                { 7, new Experience(7, 2100, 700) },
                { 8, new Experience(8, 2800, 800) },
                { 9, new Experience(9, 3600, 900) },
                { 10, new Experience(10, 4500, 1000) },
                { 11, new Experience(11, 5500, 1100) },
                { 12, new Experience(12, 6600, 1200) },
                { 13, new Experience(13, 7800, 1300) },
                { 14, new Experience(14, 9100, 1400) },
                { 15, new Experience(15, 10500, 1500) },
                { 16, new Experience(16, 12000, 1600) },
                { 17, new Experience(17, 13600, 1700) },
                { 18, new Experience(18, 15300, 1800) },
                { 19, new Experience(19, 17100, 1900) },
                { 20, new Experience(20, 19000, 2000) },
                { 21, new Experience(21, 21000, 2100) },
                { 22, new Experience(22, 23100, 2200) },
                { 23, new Experience(23, 25300, 2300) },
                { 24, new Experience(24, 27600, 2400) },
                { 25, new Experience(25, 30000, 2500) },
                { 26, new Experience(26, 32500, 2600) },
                { 27, new Experience(27, 35100, 2700) },
                { 28, new Experience(28, 37800, 2800) },
                { 29, new Experience(29, 40600, 2900) },
                { 30, new Experience(30, 43500, 3000) },
                { 31, new Experience(31, 46500, 3100) },
                { 32, new Experience(32, 49600, 3200) },
                { 33, new Experience(33, 52800, 3300) },
                { 34, new Experience(34, 56100, 3400) },
                { 35, new Experience(35, 59500, 3500) },
                { 36, new Experience(36, 63000, 3600) },
                { 37, new Experience(37, 66600, 3700) },
                { 38, new Experience(38, 70300, 3800) },
                { 39, new Experience(39, 74100, 3900) },
                { 40, new Experience(40, 78000, 4000) },
                { 41, new Experience(41, 82000, 4100) },
                { 42, new Experience(42, 86100, 4200) },
                { 43, new Experience(43, 90300, 4300) },
                { 44, new Experience(44, 94600, 4400) },
                { 45, new Experience(45, 99000, 4500) },
                { 46, new Experience(46, 103500, 4600) },
                { 47, new Experience(47, 108100, 4700) },
                { 48, new Experience(48, 112800, 4800) },
                { 49, new Experience(49, 117600, 4900) },
                { 50, new Experience(50, 122500, 5000) },
                { 51, new Experience(51, 127500, 5200) },
                { 52, new Experience(52, 132700, 5400) },
                { 53, new Experience(53, 138100, 5600) },
                { 54, new Experience(54, 143700, 5800) },
                { 55, new Experience(55, 149500, 6000) },
                { 56, new Experience(56, 155500, 6200) },
                { 57, new Experience(57, 161700, 6400) },
                { 58, new Experience(58, 168100, 6600) },
                { 59, new Experience(59, 174700, 6800) },
                { 60, new Experience(60, 181500, 7000) },
                { 61, new Experience(61, 188500, 7300) },
                { 62, new Experience(62, 195800, 7600) },
                { 63, new Experience(63, 203400, 7900) },
                { 64, new Experience(64, 211300, 8200) },
                { 65, new Experience(65, 219500, 8500) },
                { 66, new Experience(66, 228000, 8800) },
                { 67, new Experience(67, 236800, 9100) },
                { 68, new Experience(68, 245900, 9400) },
                { 69, new Experience(69, 255300, 9700) },
                { 70, new Experience(70, 265000, 10000) },
                { 71, new Experience(71, 275000, 10400) },
                { 72, new Experience(72, 285400, 10800) },
                { 73, new Experience(73, 296200, 11200) },
                { 74, new Experience(74, 307400, 11600) },
                { 75, new Experience(75, 319000, 12000) },
                { 76, new Experience(76, 331000, 12400) },
                { 77, new Experience(77, 343400, 12800) },
                { 78, new Experience(78, 356200, 13200) },
                { 79, new Experience(79, 369400, 13600) },
                { 80, new Experience(80, 383000, 14000) },
                { 81, new Experience(81, 397000, 14500) },
                { 82, new Experience(82, 411500, 15000) },
                { 83, new Experience(83, 426500, 15500) },
                { 84, new Experience(84, 442000, 16000) },
                { 85, new Experience(85, 458000, 16500) },
                { 86, new Experience(86, 474500, 17000) },
                { 87, new Experience(87, 491500, 17500) },
                { 88, new Experience(88, 509000, 18000) },
                { 89, new Experience(89, 527000, 18500) },
                { 90, new Experience(90, 545500, 19000) },
                { 91, new Experience(91, 564500, 20000) },
                { 92, new Experience(92, 584500, 22000) },
                { 93, new Experience(93, 606500, 25000) },
                { 94, new Experience(94, 631500, 30000) },
                { 95, new Experience(95, 661500, 40000) },
                { 96, new Experience(96, 701500, 60000) },
                { 97, new Experience(97, 761500, 90000) },
                { 98, new Experience(98, 851500, 148500) },
                { 99, new Experience(99, 1000000, 0) },
                { 100, new Experience(100, 1000000, 10000) },
                { 101, new Experience(101, 1010000, 1000) },
                { 102, new Experience(102, 1011000, 2000) },
                { 103, new Experience(103, 1013000, 3000) },
                { 104, new Experience(104, 1016000, 4000) },
                { 105, new Experience(105, 1020000, 5000) },
                { 106, new Experience(106, 1025000, 6000) },
                { 107, new Experience(107, 1031000, 7000) },
                { 108, new Experience(108, 1038000, 8000) },
                { 109, new Experience(109, 1046000, 9000) },
                { 110, new Experience(110, 1055000, 10000) },
                { 111, new Experience(111, 1065000, 12000) },
                { 112, new Experience(112, 1077000, 14000) },
                { 113, new Experience(113, 1091000, 16000) },
                { 114, new Experience(114, 1107000, 18000) },
                { 115, new Experience(115, 1125000, 20000) },
                { 116, new Experience(116, 1145000, 23000) },
                { 117, new Experience(117, 1168000, 26000) },
                { 118, new Experience(118, 1194000, 29000) },
                { 119, new Experience(119, 1223000, 32000) },
                { 120, new Experience(120, 1255000, 35000) },
                { 121, new Experience(121, 1290000, 39000) },
                { 122, new Experience(122, 1329000, 43000) },
                { 123, new Experience(123, 1372000, 47000) },
                { 124, new Experience(124, 1419000, 51000) },
                { 125, new Experience(125, 1470000, 55000) },
                { 126, new Experience(126, 1525000, 59000) },
                { 127, new Experience(127, 1584000, 63000) },
                { 128, new Experience(128, 1647000, 67000) },
                { 129, new Experience(129, 1714000, 71000) },
                { 130, new Experience(130, 1785000, 75000) },
                { 131, new Experience(131, 1860000, 80000) },
                { 132, new Experience(132, 1940000, 85000) },
                { 133, new Experience(133, 2025000, 90000) },
                { 134, new Experience(134, 2115000, 95000) },
                { 135, new Experience(135, 2210000, 100000) },
                { 136, new Experience(136, 2310000, 105000) },
                { 137, new Experience(137, 2415000, 110000) },
                { 138, new Experience(138, 2525000, 115000) },
                { 139, new Experience(139, 2640000, 120000) },
                { 140, new Experience(140, 2760000, 127000) },
                { 141, new Experience(141, 2887000, 134000) },
                { 142, new Experience(142, 3021000, 141000) },
                { 143, new Experience(143, 3162000, 148000) },
                { 144, new Experience(144, 3310000, 155000) },
                { 145, new Experience(145, 3465000, 163000) },
                { 146, new Experience(146, 3628000, 171000) },
                { 147, new Experience(147, 3799000, 179000) },
                { 148, new Experience(148, 3978000, 187000) },
                { 149, new Experience(149, 4165000, 195000) },
                { 150, new Experience(150, 4360000, 204000) },
                { 151, new Experience(151, 4564000, 213000) },
                { 152, new Experience(152, 4777000, 222000) },
                { 153, new Experience(153, 4999000, 231000) },
                { 154, new Experience(154, 5230000, 240000) },
                { 155, new Experience(155, 5470000, 250000) },
                { 156, new Experience(146, 5720000, 60000) },
                { 157, new Experience(147, 5780000, 80000) },
                { 158, new Experience(148, 5860000, 110000) },
                { 159, new Experience(149, 5970000, 150000) },
                { 160, new Experience(150, 6120000, 200000) },
                { 161, new Experience(151, 6320000, 260000) },
                { 162, new Experience(152, 6580000, 330000) },
                { 163, new Experience(153, 6910000, 410000) },
                { 164, new Experience(154, 7320000, 500000) },
                { 165, new Experience(155, 7820000, 0) },
            });
        }

        public static int GetAdmiralExperienceToNextLevel(int rpCurrentLevel, int rpCurrentExperience)
        {
            if (rpCurrentLevel == 120)
                return 180000000 - rpCurrentExperience;

            Experience rExperience;
            if (Admiral.TryGetValue(rpCurrentLevel + 1, out rExperience))
                return rExperience.Total - rpCurrentExperience;
            else
                return 0;
        }
        public static int GetShipExperienceToLevel(int? rpTargetLevel, int rpCurrentExperience)
        {
            Experience rExperience;
            if (rpTargetLevel.HasValue && Ship.TryGetValue(rpTargetLevel.Value, out rExperience))
                return rExperience.Total - rpCurrentExperience;
            else
                return 0;
        }
    }
}