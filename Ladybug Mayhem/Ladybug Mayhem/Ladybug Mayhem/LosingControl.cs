﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//ikke default
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ladybug_Mayhem
{
    public class LosingControl
    {
        private static ContentManager _content;

        private static List<DrawSprite> _drawHearts;
        private static List<Citizen> _citizenList;
        private static int[] _spawnTimeLevels;
        private static int _minSpawnTime;
        private static int _spawnTimer;
        private static int _populationCount;

        private static bool _alreadySavedACitizen;

        public static void Initialize(ContentManager content)
        {
            _content = content;
            _spawnTimeLevels = new int[3];
            _spawnTimeLevels[0] = 200;
            _spawnTimeLevels[1] = 2700;
            _spawnTimeLevels[2] = 3000;
            _drawHearts = new List<DrawSprite>();
            _citizenList = new List<Citizen>();
            Reset(content);
        }

        public static void Update(GameTime gameTime, GameWindow window)
        {
            _spawnTimer += gameTime.ElapsedGameTime.Milliseconds;
            _minSpawnTime = _spawnTimeLevels[GlobalVars.bugs_killed] + _spawnTimer;
            if (_minSpawnTime >= 4000 && _populationCount < GlobalVars.MAX_CITIZENS)
            {
                _citizenList.Add(new Citizen(_content, _populationCount));
                _populationCount++;
                _spawnTimer = 0;
            }
            _alreadySavedACitizen = false;
            //Denne loopen teller nedover, slik at den oppdaterer "siste" citizen først. Dersom man klikker to citizens som overlapper
            //hverandre skal bare en av dem "reddes" (sendes tilbake). Siden loopen teller nedover vil den "øverste" (/"sist innlastede")
            //citizen'en, utifra logikken, være den som reddes. Dette er mest naturlig.

            for (int citizenNumber = _citizenList.Count-1; citizenNumber >= 0; citizenNumber--)
            {    
                _citizenList[citizenNumber].Update(gameTime);
                //Sjekker om musen klikkes i denne framen og passer på at bare "øverste" (/"sist innlastede") citizen sendes tilbake
                if (CheckMousePress.IsBeingPressed(_citizenList[citizenNumber].GetCitizenBox()) && !_alreadySavedACitizen)
                {
                    _citizenList[citizenNumber].Saved(_citizenList);
                    _alreadySavedACitizen = true;
                }
                //En citizen "dør" (går ut av skjermen)
                if (_citizenList[citizenNumber].GetCitizenBox().X > window.ClientBounds.Width)
                {
                    _citizenList.RemoveAt(citizenNumber);
                    GlobalVars.lives--;
                    _drawHearts.RemoveAt(_drawHearts.Count - 1);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int citizenNumber = 0; citizenNumber < _citizenList.Count; citizenNumber++)
            {
                _citizenList[citizenNumber].Draw(spriteBatch);
            }
            for (int heartCounter = 0; heartCounter < GlobalVars.lives; heartCounter++)
            {
                _drawHearts[heartCounter].Draw(spriteBatch);
            }
        }

        public static void Reset(ContentManager content)
        {
            _citizenList.Clear();
            _citizenList.Add(new Citizen(_content, 0));
            _drawHearts.Clear();
            for (int heartCounter = 0; heartCounter < GlobalVars.MAX_LIVES; heartCounter++)
            {
                _drawHearts.Add(new DrawSprite(content, "heart",
                    new Rectangle(5 + ((GlobalVars.HEART_WIDTH_HEIGHT+12) * heartCounter),3, GlobalVars.HEART_WIDTH_HEIGHT, GlobalVars.HEART_WIDTH_HEIGHT),
                    GlobalVars.HEART_SPRITE_RECTANGLE, 1));
            }
            _populationCount = 1;
            _spawnTimer = 2000;
            GlobalVars.lives = GlobalVars.MAX_LIVES;
        }
    }
}