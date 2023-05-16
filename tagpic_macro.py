import ctypes
from functools import partial

from pyautogui import *
import pyautogui
import time
from pynput import keyboard, mouse
import keyboard as kb
import random
import win32api, win32con, win32gui, win32com.client
from pynput.keyboard import Key, Controller


def click(pos):
    win32api.SetCursorPos((pos.x, pos.y))
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0)
    time.sleep(0.06)  # This pauses the script for 0.1 seconds
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0)

ctypes.windll.kernel32.SetConsoleTitleW("bg afk")

def click(pos):
    win32api.SetCursorPos((pos.x, pos.y))
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0)
    time.sleep(0.1)  # This pauses the script for 0.1 seconds
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0)

def window(window):
    def windowEnumerationHandler(hwnd, top_windows):
        top_windows.append((hwnd, win32gui.GetWindowText(hwnd)))

    results = []
    top_windows = []
    win32gui.EnumWindows(windowEnumerationHandler, top_windows)
    for i in top_windows:
        if window == i[1].lower():
            keyboard.send('alt')
            win32gui.ShowWindow(i[0], 5)
            win32gui.SetForegroundWindow(i[0])
            return
    for i in top_windows:
            if window in i[1].lower():
                keyboard.send('alt')
                win32gui.ShowWindow(i[0], 5)
                win32gui.SetForegroundWindow(i[0])
                return

def on_press(key):
    # print(key)
    if key == Key.pause:
        copy_and_save()


f = False

def copy_and_save():
    copy()
    time.sleep(0.1)
    save()
    while win32gui.GetWindowText(win32gui.GetForegroundWindow()) != 'ImageConfirmationForm':
        time.sleep(0.1)
    click(777, 777) # КОСТЫЛЬ ДЛЯ АКТИВАЦИИ НОВОГО ОКНА


def copy():

    win32api.mouse_event(win32con.MOUSEEVENTF_RIGHTDOWN, 0, 0)
    time.sleep(0.1)  # This pauses the script for 0.1 seconds
    win32api.mouse_event(win32con.MOUSEEVENTF_RIGHTUP, 0, 0)
    time.sleep(0.1)
    # print('pressing right button')

    pos = pyautogui.position()
    x = getX(pos.x)
    y = getY(pos.y)
    click(position(x, y))
    # print('copying finished')

def getX(x):
    if x <= 1371:
        return x + 217
    return x - 217

def getY(y):
    if y <= 508:
        return y + 111
    return y - 377


def save():
    keyboard = Controller()
    keyboard.press('`')
    keyboard.release('`')




klistener = keyboard.Listener(on_press=on_press)
klistener.start()  # start to listen on a separate thread
klistener.join()
