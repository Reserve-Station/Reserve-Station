// SPDX-FileCopyrightText: 2025 Kutosss <162154227+Kutosss@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server.RandomMetadata;


/// <summary>
/// This is used for excluding specific entities from receiving a RandomMetadata pre-spawn.
/// </summary>
[RegisterComponent]
public sealed partial class RandomMetadataExcludedComponent : Component { }
