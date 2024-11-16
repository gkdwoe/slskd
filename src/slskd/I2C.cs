// <copyright file="I2C.cs" company="slskd Team">
//     Copyright (c) slskd Team. All rights reserved.
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as published
//     by the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
//
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see https://www.gnu.org/licenses/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.CharacterLcd;
using Iot.Device.Pcx857x;

namespace I2C
{
    public class I2CDriver : IDisposable
    {
        public static I2CDriver Instance { get; private set; }
        private const int Address = 0x27;
        private Queue<Tuple<string, bool>> messages = new Queue<Tuple<string, bool>>();
        private Task messageTask;

        private I2cDevice device;
        private Pcf8574 driver;
        private Lcd2004 lcd;

        private I2CDriver()
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(1, Address);
            device = I2cDevice.Create(settings);
            driver = new Pcf8574(device);
            lcd = new Lcd2004(
                registerSelectPin: 0,
                enablePin: 2,
                dataPins: new int[] { 4, 5, 6, 7 },
                backlightPin: 3,
                backlightBrightness: 0.1f,
                readWritePin: 1,
                controller: new GpioController(PinNumberingScheme.Logical, driver));

            lcd.Clear();

            messageTask = Task.Run(() => ProcessQueue());
        }

        public static I2CDriver GetInstance()
        {
            if (Instance == null)
            {
                Instance = new I2CDriver();
            }

            return Instance;
        }

        public void QueueMessage(string message, bool success)
        {
            messages.Enqueue(Tuple.Create(message, success));
        }

        private async Task ProcessQueue()
        {
            while (true)
            {
                string? message = null;
                bool status = false;

                lock (messages)
                {
                    if (messages.Count > 0)
                    {
                        var item = messages.Dequeue();
                        message = item.Item1;
                        status = item.Item2;
                    }
                }

                if (message != null)
                {
                    WriteToLcd(message, status);

                    await Task.Delay(1000);
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public void WriteToLcd(string message, bool success)
        {
            string status = success ? "status: success" : "status: failed";
            int max = 16;

            if (message.Length > max)
            {
                lcd.Clear();
                lcd.SetCursorPosition(0, 0);

                for (int i = 0; i <= message.Length - max; i++)
                {
                    string msg = message.Substring(i, max);

                    lcd.Clear();
                    lcd.SetCursorPosition(0, 0);
                    lcd.Write(msg);

                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(status);

                    if (i == message.Length - max && messages.Count == 0)
                    {
                        i = -1;
                    }

                    Thread.Sleep(450);
                }
            }
            else
            {
                lcd.Clear();
                lcd.SetCursorPosition(0, 0);
                lcd.Write(message);

                lcd.SetCursorPosition(0, 1);
                lcd.Write(status);
            }
        }

        public void Dispose()
        {
            lcd.Clear();
            driver?.Dispose();
            lcd?.Dispose();
            messageTask?.Dispose();
        }
    }
}
