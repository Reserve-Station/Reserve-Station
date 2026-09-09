// SPDX-License-Identifier: MIT

using Content.Shared.Stylesheets; // Reserve edit: ReserveRedStyle

namespace Content.Shared.Chat;

public static class ChatChannelExtensions
{
    public static Color TextColor(this ChatChannel channel)
    {
        return channel switch
        {
            ChatChannel.Server => Color.Goldenrod, // Reserve edit: ReserveRedStyle
            ChatChannel.Radio => Color.LimeGreen,
            ChatChannel.LOOC => Color.MediumTurquoise,
            ChatChannel.OOC => Color.LightSkyBlue,
            ChatChannel.Dead => Color.MediumPurple,
            ChatChannel.Admin => ReserveRed.Red, // Reserve edit: ReserveRedStyle
            ChatChannel.AdminAlert => ReserveRed.DarkRed, // Reserve edit: ReserveRedStyle
            ChatChannel.AdminChat => Color.HotPink,
            ChatChannel.Whisper => Color.DarkGray,
            _ => Color.LightGray
        };
    }
}
