using System;
using System.Collections.Generic;
using Past.Common.Database.Record;
using Past.Common.Utils;
using Past.Protocol.Enums;
using Past.Protocol.Types;

namespace Past.Game.Helper
{
    public class GeneralHelper
    {
        public static EntityLook BuildEntityLook(string entityLook, CharacterRecord character) //TODO SubEntity
        {
            var look_string = entityLook.Replace("{", "").Replace("}", "").Split('|');
            var bonesId = short.Parse(look_string[0]);
            short[] skins;
            if (look_string[1].Contains(","))
            {
                var skins_string = look_string[1].Split(',');
                skins = new short[skins_string.Length];
                for (var i = 0; i < skins_string.Length; i++)
                {
                    skins[i] = short.Parse(skins_string[i]);
                }
            }
            else
            {
                skins = string.IsNullOrEmpty(look_string[1]) ? new short[0] : new[] { short.Parse(look_string[1]) };
            }
            int[] colors;
            if (look_string[2].Contains(","))
            {
                var colors_string = look_string[2].Split(',');
                colors = new int[colors_string.Length];
                for (var i = 0; i < colors_string.Length; i++)
                {
                    var color = int.Parse(colors_string[i].Remove(0, 2));
                    if (color != -1)
                    {
                        colors[i] = (i + 1 & 255) << 24 | color & 16777215;
                    }
                }
            }
            else
            {
                colors = new int[0];
            }
            var size = new[] { short.Parse(look_string[3]) };
            return new EntityLook(bonesId, skins, colors, size, GetSubEntity(character));
            //return new EntityLook(bonesId, skins, colors, size, new SubEntity[0]);
        }

        public static SubEntity[] GetSubEntity(CharacterRecord character)
        {
            try
            {
                var returnSubEntity = new List<SubEntity>();

                if (character.Direction == (DirectionsEnum)2 && character.Level >= 100 && character.Level != 200)
                {
                    var sub = new SubEntity()
                    {
                        bindingPointCategory = 6,
                        bindingPointIndex = 0,
                        subEntityLook = new EntityLook(169, new short[0], new int[0], new short[0], new SubEntity[0])
                    };
                    returnSubEntity.Add(sub);
                }
                else if (character.Direction == (DirectionsEnum)2 && character.Level == 200)
                {
                    var sub = new SubEntity()
                    {
                        bindingPointCategory = 6,
                        bindingPointIndex = 0,
                        subEntityLook = new EntityLook(170, new short[0], new int[0], new short[0], new SubEntity[0])
                    };
                    returnSubEntity.Add(sub);
                }

                return returnSubEntity.ToArray();
            }
            catch (Exception ex)
            {
                ConsoleUtils.Write(ConsoleUtils.Type.ERROR, ex.Message);
                return new SubEntity[0];
            }
        }
    }
}
