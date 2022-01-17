﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace SpaceInvaders
{
    class EnemyBlock : GameObject
    {
        private HashSet<SpaceShip> _enemyShips;
        private int _baseWidth;
        private int _bottomShipMargin;
        private Size _size;
        private Vector2 _position;
        private int _direction;
        private double _horSpeed;
        private double _verSpeed;
        private double _randomShootProbability;

        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public EnemyBlock(Vector2 spawnPos, int blockWidth, int direction)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _bottomShipMargin = 16;
            _size.Width = blockWidth;
            _size.Height = _bottomShipMargin * -1;
            _baseWidth = _size.Width;
            _enemyShips = new HashSet<SpaceShip>();
            _direction = direction;
            _side = Side.Enemy;
            _randomShootProbability = 0.01;
            _horSpeed = 50;
            _verSpeed = 4000;
        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // Si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
                if (_position.X + _size.Width >= gameInstance.gameSize.Width)
                {
                    _direction = -1; // goes Left

                    _position.Y += _verSpeed * deltaT;
                    _horSpeed += 2;
                    foreach (SpaceShip spaceship in _enemyShips)
                    {
                      spaceship.Position.Y += _verSpeed * deltaT;
                      _randomShootProbability += 0.002;
                    }

                }
                else if (_position.X <= 0)
                {
                    _direction = 1; // goes Right

                    _position.Y += _verSpeed * deltaT;
                    _horSpeed += 2;
                    foreach (SpaceShip spaceship in _enemyShips)
                    {
                       spaceship.Position.Y += _verSpeed * deltaT;
                       _randomShootProbability += 0.002;
                    }
                }


                foreach (SpaceShip spaceship in _enemyShips.ToList())
                {
                    spaceship.Position.X += _horSpeed * deltaT * _direction;

                    Random randSeed = new Random();
                    double r = randSeed.NextDouble();

                    if (spaceship.IsAlive() == false)
                    {
                        _enemyShips.Remove(spaceship);
                    }
                    else if (r <= _randomShootProbability * deltaT) //random shoot
                    {
                        spaceship.Shoot(gameInstance);
                    }
                }
                _position.X += _horSpeed * deltaT * _direction;


                if (_position.Y + _size.Height >= gameInstance.playerShip.Position.Y)
                {
                    gameInstance.playerShip.Kill();
                }
            }
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            foreach (SpaceShip spaceship in _enemyShips)
            {
                spaceship.Draw(gameInstance, graphics);
            }

            Pen pen = new Pen(Color.Red, 2);
            int x = (int)_position.X;
            int y = (int)_position.Y;

            graphics.DrawLine(pen, x, y, x + _size.Width, y);
            graphics.DrawLine(pen, x, y, x, _size.Height + y);
            graphics.DrawLine(pen, x + _size.Width, y, x + _size.Width, _size.Height + y);
            graphics.DrawLine(pen, x, y + _size.Height, x + _size.Width, y + _size.Height);
        }

        public override bool IsAlive()
        {
            foreach(SpaceShip spaceship in _enemyShips)
            {
                if (spaceship.IsAlive())
                {
                    return true;
                }
            }
            return false;
        }

        public override void Collision(Missile m)
        {
            foreach (SpaceShip spaceship in _enemyShips)
            {
                spaceship.Collision(m);
            }
        }

        public void AddLine(int nbShips, int nbLives, Bitmap shipImage)
        {
            for (int ship = 0; ship < nbShips; ship++)
            {
                SpaceShip enemyship = new SpaceShip(new Vector2(_position.X + (_baseWidth / nbShips * (ship + 1)) - (_baseWidth / nbShips) + (_baseWidth / (nbShips * 2)) - (shipImage.Width/2), _position.Y + _size.Height + _bottomShipMargin), shipImage, nbLives, Side.Enemy);
                _enemyShips.Add(enemyship);
            }
            UpdateSize(shipImage);
        }

        private void UpdateSize(Bitmap shipImg)
        {
            _size.Height += shipImg.Height + _bottomShipMargin;
        }
    }
}
