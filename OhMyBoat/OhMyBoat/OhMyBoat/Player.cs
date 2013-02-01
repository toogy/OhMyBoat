﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OhMyBoat.IO;
using OhMyBoat.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OhMyBoat
{
    public class Player
    {
        public string Name { get; private set; }

        public int Shots;

        public Map Map { get; set; }

        public Player(string name) : this(name, Map.Generate()) { }

        public Player(string name, Map map)
        {
            Name = name;
            Map = map;
            Shots = 0;
            Map.Aim = new Point(0, 0);
        }

        public string Play(int x, int y)
        {
            if (!Shoot(x, y))
            {
                return "You missed the shot... I shall not say that you deeply are a sucker.";
            }

            if (Sink(x, y))
            {
                Achieve(x, y);
                return "You hit the target and you destroyed a boat! You can be pride of yourself.";
            }

            return "You hit the target! Well done!";
        }

        public void Update()
        {
            if (GameDatas.KeyboardFocus)
            {
                if (GameDatas.PreviousKeyboardState.IsKeyDown(Keys.Left) &&
                     (GameDatas.KeyboardState.IsKeyUp(Keys.Left) || Map.AimPeriod == GameDatas.MapPeriod))
                {
                    Map.Aim.X = (Map.Aim.X - 1) < 0 ? (GameDatas.GridTheme.CellsNumber - 1) : (Map.Aim.X - 1);
                    Map.AimPeriod = 0;
                }

                if (GameDatas.PreviousKeyboardState.IsKeyDown(Keys.Right) &&
                     (GameDatas.KeyboardState.IsKeyUp(Keys.Right) || Map.AimPeriod == GameDatas.MapPeriod))
                {
                    Map.Aim.X = (Map.Aim.X + 1) > (GameDatas.GridTheme.CellsNumber - 1) ? 0 : (Map.Aim.X + 1);
                    Map.AimPeriod = 0;
                }

                if (GameDatas.PreviousKeyboardState.IsKeyDown(Keys.Up) &&
                     (GameDatas.KeyboardState.IsKeyUp(Keys.Up) || Map.AimPeriod == GameDatas.MapPeriod))
                {
                    Map.Aim.Y = (Map.Aim.Y - 1) < 0 ? (GameDatas.GridTheme.CellsNumber - 1) : (Map.Aim.Y - 1);
                    Map.AimPeriod = 0;
                }

                if (GameDatas.PreviousKeyboardState.IsKeyDown(Keys.Down) &&
                     (GameDatas.KeyboardState.IsKeyUp(Keys.Down) || Map.AimPeriod == GameDatas.MapPeriod))
                {
                    Map.Aim.Y = (Map.Aim.Y + 1) > (GameDatas.GridTheme.CellsNumber - 1) ? 0 : (Map.Aim.Y + 1);
                    Map.AimPeriod = 0;
                }

                if (GameDatas.KeyboardState.IsKeyDown(Keys.Right) || GameDatas.KeyboardState.IsKeyDown(Keys.Down) ||
                    GameDatas.KeyboardState.IsKeyDown(Keys.Up) || GameDatas.KeyboardState.IsKeyDown(Keys.Left))
                    Map.AimPeriod++;
            }

            else
            {
                if (Map.Area.Contains(GameDatas.MouseState.X, GameDatas.MouseState.Y))
                {
                    Map.Aim.X = (GameDatas.MouseState.X - Map.X - GameDatas.GridTheme.GridPadding) / GameDatas.GridTheme.CellSize;
                    Map.Aim.Y = (GameDatas.MouseState.Y - Map.Y - GameDatas.GridTheme.GridPadding) / GameDatas.GridTheme.CellSize;
                }
            }
        }

        public bool IsOver()
        {
            for (var i = 0; i < GameDatas.GridTheme.CellsNumber; i++)
                for (var j = 0; j < GameDatas.GridTheme.CellsNumber; j++)
                    if (Map.Datas[i, j] == (byte) CellState.BoatHidden)
                        return false;

            return true;
        }

        public bool Shoot(int x, int y)
        {
            switch (Map.Datas[x, y])
            {
                case (byte)CellState.BoatHidden:
                    Shots++;
                    Map.Datas[x, y] = (byte)CellState.BoatBurning;
                    return true;
                case (byte)CellState.WaterHidden:
                    Shots++;
                    Map.Datas[x, y] = (byte)CellState.Water;
                    return false;
                default:
                    return false;
            }
        }

        public bool Sink(int x, int y)
        {
            return Sink(x - 1, y, -1, 0) && Sink(x + 1, y, 1, 0) && Sink(x, y - 1, 0, -1) && Sink(x, y + 1, 0, 1);
        }

        public bool Sink(int x, int y, int dirx, int diry)
        {
            return (x < 0 || y < 0 || x > (GameDatas.GridTheme.CellsNumber - 1) || y > (GameDatas.GridTheme.CellsNumber - 1) || Map.Datas[x, y] == (byte)CellState.WaterHidden || Map.Datas[x, y] == (byte)CellState.Water) || (Map.Datas[x, y] == (byte)CellState.BoatBurning && Sink(x + dirx, y + diry, dirx, diry));
        }

        public void Achieve(int x, int y)
        {
            Achieve(x, y, 0, 0);
            Achieve(x - 1, y, -1, 0);
            Achieve(x + 1, y, 1, 0);
            Achieve(x, y - 1, 0, -1);
            Achieve(x, y + 1, 0, 1);
        }

        public bool Achieve(int x, int y, int dirx, int diry)
        {
            if (x < 0 || y < 0 || x > (GameDatas.GridTheme.CellsNumber - 1) || y > (GameDatas.GridTheme.CellsNumber - 1) || Map.Datas[x, y] == (byte)CellState.WaterHidden || Map.Datas[x, y] == (byte)CellState.Water)
            {
                return true;
            }

            if (Map.Datas[x, y] == (byte) CellState.BoatBurning && (dirx == diry || Achieve(x + dirx, y + diry, dirx, diry)))
            {
                Map.Datas[x, y] = (byte) CellState.BoatDestroyed;

                if (x - 1 >= 0 && Map.Datas[x - 1, y] == (byte) CellState.WaterHidden)
                    Map.Datas[x - 1, y] = (byte) CellState.Water;

                if (x + 1 < GameDatas.GridTheme.CellsNumber && Map.Datas[x + 1, y] == (byte)CellState.WaterHidden)
                    Map.Datas[x + 1, y] = (byte) CellState.Water;

                if (y - 1 >= 0 && Map.Datas[x, y - 1] == (byte)CellState.WaterHidden)
                    Map.Datas[x, y - 1] = (byte) CellState.Water;

                if (y + 1 < GameDatas.GridTheme.CellsNumber && Map.Datas[x, y + 1] == (byte)CellState.WaterHidden)
                    Map.Datas[x, y + 1] = (byte) CellState.Water;

                return true;
            }

            return false;
        }
    }
}
