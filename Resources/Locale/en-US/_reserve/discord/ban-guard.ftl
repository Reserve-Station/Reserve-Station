# Reserve Red - BanGuard: Discord logs for bans, unbans and notes

ban-guard-title-server-ban = Ban | ID { $id }
ban-guard-title-role-ban = Role ban | ID { $id }
ban-guard-title-server-unban = Unban | ID { $id }
ban-guard-title-role-unban = Role ban removed | ID { $id }
ban-guard-title-note-added = Added: { $type } #{ $id }
ban-guard-title-note-modified = Modified: { $type } #{ $id }
ban-guard-title-note-deleted = Deleted: { $type } #{ $id }
ban-guard-title-first-join = New player on the server
ban-guard-title-exemption = Ban exemption changed
ban-guard-title-job-whitelist-added = Job whitelist added
ban-guard-title-job-whitelist-removed = Job whitelist removed
ban-guard-user-id = **UserID:** { $userId }
ban-guard-exemption-changed = **Exemption flags:** { $old } → { $new }
ban-guard-whitelist-job = **Job:** { $job }

ban-guard-target = **Player:** { $target }
ban-guard-target-no-user = *no linked account (IP/HWID)*
ban-guard-reason = **Reason:** { $reason }
ban-guard-duration = **Duration:** { $days } d. { $hours } h. { $minutes } m.
ban-guard-permanent = **Duration:** ***Permanent***
ban-guard-expires = **Expires:** { $expires }
ban-guard-banned-by = **Issued by:** { $admin }
ban-guard-unbanned-by = **Removed by:** { $admin }
ban-guard-new-roles = **Newly banned roles:** { $roles }
ban-guard-all-roles = **All active role bans ({ $target }):** { $roles }
ban-guard-pardoned-roles = **Roles:** { $roles }
ban-guard-none = *none*
ban-guard-never = *never*

ban-guard-note-message = **Text:** { $message }
ban-guard-note-severity = **Severity:** { $severity }
ban-guard-note-secret = **Secret:** { $secret }
ban-guard-note-expiry = **Expires:** { $expiry }
ban-guard-note-by = **Admin:** { $admin }
ban-guard-note-type-note = Note
ban-guard-note-type-watchlist = Watchlist
ban-guard-note-type-message = Message
ban-guard-note-type-server-ban = Ban
ban-guard-note-type-role-ban = Role ban
ban-guard-note-message-changed = **Text:** { $old } → { $new }
ban-guard-note-severity-changed = **Severity:** { $old } → { $new }
ban-guard-note-secret-changed = **Secret:** { $old } → { $new }
ban-guard-note-expiry-changed = **Expiry:** { $old } → { $new }
