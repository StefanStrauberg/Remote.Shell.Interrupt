import { useState } from "react";
import {
  Chip,
  IconButton,
  MenuItem,
  Select,
  SelectChangeEvent,
  Switch,
  TableCell,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import { Delete } from "@mui/icons-material";
import { toast } from "react-toastify";
import { format } from "date-fns";
import { User } from "@/lib/types/Users/User";
import { ASSIGNABLE_ROLES } from "@/lib/types/Users/User";
import {
  useDeleteUserMutation,
  useSetUserActiveMutation,
  useUpdateUserRoleMutation,
} from "../api/usersQueries";

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}

type Props = {
  user: User;
  isSelf: boolean;
};

export default function UserRow({ user, isSelf }: Props) {
  const updateRole = useUpdateUserRoleMutation();
  const setActive = useSetUserActiveMutation();
  const deleteUser = useDeleteUserMutation();
  const [pendingActive, setPendingActive] = useState<boolean | null>(null);

  const primaryRole = user.roles[0] ?? ASSIGNABLE_ROLES[1];
  const isBusy = updateRole.isPending || setActive.isPending || deleteUser.isPending;

  const handleRoleChange = (event: SelectChangeEvent<string>) => {
    const role = event.target.value;
    updateRole.mutate(
      { userId: user.id, role },
      {
        onSuccess: () => toast.success(`${user.email} is now ${role}.`),
        onError: (error) =>
          toast.error(`Failed to change role: ${errorMessage(error)}`),
      }
    );
  };

  const handleToggleActive = () => {
    const nextActive = !user.isActive;
    setPendingActive(nextActive);
    setActive.mutate(
      { userId: user.id, isActive: nextActive },
      {
        onSuccess: () =>
          toast.success(
            `${user.email} was ${nextActive ? "activated" : "deactivated"}.`
          ),
        onError: (error) =>
          toast.error(`Failed to update account: ${errorMessage(error)}`),
        onSettled: () => setPendingActive(null),
      }
    );
  };

  const handleDelete = () => {
    if (!window.confirm(`Permanently delete "${user.email}"? This cannot be undone.`))
      return;

    deleteUser.mutate(user.id, {
      onSuccess: () => toast.success(`${user.email} was deleted.`),
      onError: (error) => toast.error(`Failed to delete user: ${errorMessage(error)}`),
    });
  };

  const displayActive = pendingActive ?? user.isActive;

  return (
    <TableRow hover>
      <TableCell>
        <Typography variant="body2" fontWeight={isSelf ? 700 : 400}>
          {user.email}
        </Typography>
      </TableCell>
      <TableCell>{user.fullName || "—"}</TableCell>
      <TableCell>
        {isSelf ? (
          <Chip label={primaryRole} size="small" color="primary" variant="outlined" />
        ) : (
          <Select
            value={ASSIGNABLE_ROLES.includes(primaryRole as (typeof ASSIGNABLE_ROLES)[number]) ? primaryRole : ""}
            onChange={handleRoleChange}
            size="small"
            disabled={isBusy}
          >
            {ASSIGNABLE_ROLES.map((role) => (
              <MenuItem key={role} value={role}>
                {role}
              </MenuItem>
            ))}
          </Select>
        )}
      </TableCell>
      <TableCell>
        <Tooltip title={isSelf ? "You can't deactivate your own account" : ""}>
          <span>
            <Switch
              checked={displayActive}
              onChange={handleToggleActive}
              disabled={isSelf || isBusy}
              color="success"
            />
          </span>
        </Tooltip>
        <Chip
          label={displayActive ? "Active" : "Deactivated"}
          size="small"
          color={displayActive ? "success" : "default"}
        />
      </TableCell>
      <TableCell>{format(new Date(user.createdAtUtc), "PP")}</TableCell>
      <TableCell align="right">
        {isSelf ? (
          <Chip label="You" size="small" />
        ) : (
          <Tooltip title="Delete account">
            <span>
              <IconButton
                color="error"
                onClick={handleDelete}
                disabled={isBusy}
                size="small"
              >
                <Delete fontSize="small" />
              </IconButton>
            </span>
          </Tooltip>
        )}
      </TableCell>
    </TableRow>
  );
}
