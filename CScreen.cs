﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SDL2;

namespace VisualStimuli
{
    class CScreen
    {
        // Attributes (from "struct screen" in Screen.h)
        private IntPtr m_pWindow = IntPtr.Zero; // ex "pWindow"
        private IntPtr m_pRenderer = IntPtr.Zero; // ex "renderer"
        private IntPtr m_pSurface = IntPtr.Zero; // ex "surface"
        private IntPtr m_pTexture = IntPtr.Zero; // ex "texture"
        private IntPtr m_pImage = IntPtr.Zero;// ex "image"
        private int m_x = 0;
        private int m_y = 0;
        private int m_w = 0;
        private int m_h = 0;
        private String m_name = ""; // ex "name"

        public IntPtr PWindow { get => m_pWindow; set => m_pWindow = value; }
        public IntPtr PRenderer { get => m_pRenderer; set => m_pRenderer = value; }
        public IntPtr PSurface { get => m_pSurface; set => m_pSurface = value; }
        public IntPtr PTexture { get => m_pTexture; set => m_pTexture = value; }
        public IntPtr PImage { get => m_pImage; set => m_pImage = value; }
        public int X { get => m_x; set => m_x = value; }
        public int Y { get => m_y; set => m_y = value; }
        public int W { get => m_w; set => m_w = value; }
        public int H { get => m_h; set => m_h = value; }
        public string Name { get => m_name; set => m_name = value; }


        public CScreen(int x, int y, int width, int height, String name, bool fixedScreen)
        {
            create(x, y, width, height, name, fixedScreen);
        }


        /*
        @Name: create
        @Arguement: 
            + x,y : Position of the top left corner flicker
            + width, height: flicker rectangle
            + name: 
            + fixedScrenn:
        @TODO: Create a rectangle screen no border with the defined above informations (x, y, width, height, name)
         */
        private void create(int x, int y, int width, int height, String name, bool fixedScreen)
        {
            if (!fixedScreen) {

                m_pWindow = SDL.SDL_CreateWindow(name, 
                    x,
                    y,
                    width,
                    height,
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);

                if (m_pWindow == IntPtr.Zero) {
                    Console.WriteLine("Window could not be created - Level 1 ! SDL_Error: {0}", SDL.SDL_GetError());
                    return;
                }
            }
            else {

                m_pWindow = SDL.SDL_CreateWindow(name, 
                    x,
                    y,
                    width,
                    height,
                    SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN);

                if (m_pWindow == IntPtr.Zero) {
                    Console.WriteLine("Window could not be created - Level 2 ! SDL_Error: {0}", SDL.SDL_GetError());
                    return;
                }
            }

            // the Surface
            m_pSurface = SDL.SDL_GetWindowSurface(m_pWindow);
            if (m_pSurface == IntPtr.Zero) {
                Console.WriteLine("Surface could not be created ! SDL_Error: {0}", SDL.SDL_GetError());
                return;
            }

            // the renderer
            m_pRenderer = SDL.SDL_CreateRenderer(m_pWindow, -1,SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
                                                                      SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (m_pRenderer == IntPtr.Zero) {
                Console.WriteLine("Renderer could not be created ! SDL_Error: {0}", SDL.SDL_GetError());
                return;
            }
             
            //We initialyze the screen like a black box
            var surfTmp = Marshal.PtrToStructure<SDL.SDL_Surface>(m_pSurface);
            SDL.SDL_FillRect(m_pSurface, IntPtr.Zero, SDL.SDL_MapRGB(surfTmp.format, 0, 0, 0));

            
            // the Texture
             m_pTexture = SDL.SDL_CreateTexture(m_pRenderer,
              SDL.SDL_PIXELFORMAT_ARGB8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
               width, height);

             if (m_pTexture == IntPtr.Zero) {
                Console.WriteLine("Texture could not be created ! SDL_Error: {0}", SDL.SDL_GetError());
                return;
             }
            

            // Attributes
            X = x;
            Y = y;
            W = width;
            H = height;
            Name = name;
        }

        /*
        @Name: show
        @Arguement: 
        @TODO: Supporting function of the function dislay in CFlicer.cs - Display a window
         */
        public void show()
        {
            SDL.SDL_RenderPresent(PRenderer);// how can we reaplce this function ??? 
        } 

        public void changeColorAndAlpha(UInt32 col, double alph)
        {
            changeColor(col);
            changeAlpha(alph);
        }

        /*
        @Name: changeColor
        @Arguement: 
            + col: number which express the color of flicker
            
        @TODO: Reseting the color of flicker 
         */
        public void changeColor(UInt32 col)
        {
            SDL.SDL_FillRect(PSurface, IntPtr.Zero, col); 
           
            var surfTmp = Marshal.PtrToStructure<SDL.SDL_Surface>(PSurface); 
            
            SDL.SDL_UpdateTexture(PTexture, IntPtr.Zero, surfTmp.pixels, surfTmp.pitch);

            SDL.SDL_RenderClear(PRenderer);
            SDL.SDL_RenderCopy(PRenderer, PTexture, IntPtr.Zero, IntPtr.Zero);
        }

        /*
       @Name: changeAlpha
       @Arguement: 
           + alpha: number express the opacity of flicker (0-1)

       @TODO: Setting the opacity of flicker
        */
        public void changeAlpha(double alph)
        {
            if (alph >= 0)
            {
                SDL.SDL_SetWindowOpacity(PWindow, (float)alph);
            }
        } 

    } // class CScreen

} // namespace
