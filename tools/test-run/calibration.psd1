@{
    # Every click point and every screen check box of the test-run scripts, in one place. Read by common.ps1
    # (Get-Calibration). All numbers are in the 1280 px wide screenshot of tf3ctl.ps1.
    #
    # A set is keyed '<platform>-<height of the screenshot>': windows or macos, for example 720 (16:9) or 804 (a
    # macOS game window below the menu bar).
    # The height is picked from the screenshot or the window, so another aspect ratio needs its own set.
    # $null means "not calibrated": a script that needs the value stops with a message naming it, and never
    # clicks a guess. To fill in a set, follow "Calibrating" in tools/test-run/README.md. Explicit parameters
    # (-QuitPoint, -EyePoint, ...) always win over the table.
    #
    # Points (x, y):  LoadGameTile  the main menu tile "Load Game"
    #                 Filter        the filter box of the Load Game screen
    #                 SaveCard      the first card's Load Game button
    #                 StartGame     "Start Game" on the loading screen
    #                 MenuButton    the game menu button at the bottom right
    #                 Quit, ToDesktop       "Quit" in the game menu, "Return to Desktop" in its flyout
    #                 EditorQuit, EditorToDesktop   the same in the map editor's menu (one more entry)
    #                 Exit          the exit icon of the main menu
    #                 Eye           the photo mode icon
    # Screen: the boxes (inclusive, x0..x1 by y0..y1) and thresholds of screen.ps1; see the functions there.
    #                 DarkPanel     what counts as a flat dark grey panel
    #                 DownloadBar   the Mod Hub download bar (rows Y0..Y1, columns X0..X1 every XStep)
    #                 GameMenu      the first four panels of the game menu and their icons (RowYs = panel centres)
    #                 QuitFlyout    the two dark buttons beside Quit (rows of Points.Quit)
    #                 MainMenu      the four icons at the top right (IconXs = x0, x1 of each)
    #                 PhotoMode     the three dark patches in the photo mode hint box

    # Measured on build 40423; checked unchanged on build 40425 on 2026-10-24, all but the editor points and
    # DownloadBar (no Mod Hub download ran).
    'windows-720' = @{
        Points = @{
            LoadGameTile    = @(370, 627)
            Filter          = @(180, 112)
            SaveCard        = @(176, 322)
            StartGame       = @(1192, 653)
            MenuButton      = @(1259, 686)
            Quit            = @(240, 485)
            ToDesktop       = @(788, 485)
            EditorQuit      = @(240, 517)
            EditorToDesktop = @(788, 517)
            Exit            = @(1188, 50)
            Eye             = @(1006, 710)
        }
        Screen = @{
            DarkPanel   = @{ MaxHigh = 70; MaxSpread = 14; MaxDeviation = 8 }
            DownloadBar = @{
                X0 = 82; X1 = 298; XStep = 4; Y0 = 100; Y1 = 130
                RowFraction = 0.6; MinRows = 6
                Track = @(46, 54, 61); TrackTolerance = 6
                FillMinBlue = 170; FillBlueOverRed = 80
            }
            GameMenu    = @{
                RowYs = @(235, 297, 360, 422); HalfHeight = 12
                PanelX0 = 300; PanelX1 = 400; IconX0 = 160; IconX1 = 190; MinIconBright = 25
            }
            QuitFlyout  = @{ HalfHeight = 10; Button1X0 = 430; Button1X1 = 470; Button2X0 = 690; Button2X1 = 730 }
            MainMenu    = @{ IconXs = @(@(1064, 1094), @(1101, 1131), @(1138, 1166), @(1173, 1203)); Y0 = 36; Y1 = 64; MinBright = 35 }
            PhotoMode   = @{
                # x0, x1, y0, y1 of each patch
                Boxes = @(@(1215, 1255, 646, 664), @(1215, 1255, 671, 690), @(1183, 1200, 646, 664))
                MaxHigh = 62; MaxSpread = 16
            }
        }
    }

    # Measured on 2026-10-24 (macOS 27, build 40425, English UI). The editor points are from older runs, not
    # measured again. DownloadBar is not calibrated: no Mod Hub download ran during the measuring.
    'macos-804' = @{
        Points = @{
            LoadGameTile    = @(372, 670)
            Filter          = @(180, 153)
            SaveCard        = @(177, 366)
            StartGame       = @(1192, 736)
            MenuButton      = @(1259, 769)
            Quit            = @(240, 527)
            ToDesktop       = @(788, 527)
            EditorQuit      = @(240, 558)
            EditorToDesktop = @(788, 558)
            Exit            = @(1188, 50)
            Eye             = @(1007, 794)
        }
        Screen = @{
            # Panels measured 43..53, spread up to 8, deviation up to 2
            DarkPanel   = @{ MaxHigh = 70; MaxSpread = 14; MaxDeviation = 8 }
            DownloadBar = $null
            GameMenu    = @{
                RowYs = @(277, 339, 402, 464); HalfHeight = 12
                PanelX0 = 300; PanelX1 = 400; IconX0 = 160; IconX1 = 190; MinIconBright = 25
            }
            QuitFlyout  = @{ HalfHeight = 10; Button1X0 = 430; Button1X1 = 470; Button2X0 = 690; Button2X1 = 730 }
            # Icons 35 or more (the stars are thin), the game menu's text at most 22
            MainMenu    = @{ IconXs = @(@(1064, 1094), @(1101, 1131), @(1138, 1166), @(1173, 1203)); Y0 = 36; Y1 = 64; MinBright = 28 }
            PhotoMode   = @{
                # x0, x1, y0, y1 of each patch; measured 44..45, spread 7; the HUD bar under the second one in game 53,
                # spread 14; the loading screen's black band 0..23
                Boxes = @(@(1207, 1228, 729, 745), @(1206, 1250, 755, 759), @(1210, 1250, 769, 773))
                MinHigh = 30; MaxHigh = 60; MaxSpread = 11
            }
        }
    }
}
