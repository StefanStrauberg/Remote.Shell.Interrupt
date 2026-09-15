import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import TextInput from "@/app/shared/components/TextInput";
import {
  editUserProfileSchema,
  EditUserProfileValues,
} from "@/lib/schemas/UserSchema";
import { User } from "@/lib/types/Users/User";
import { useUpdateUserProfileMutation } from "../api/usersQueries";
import { toast } from "react-toastify";

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}

type Props = {
  user: User;
  open: boolean;
  onClose: () => void;
};

export default function EditUserDialog({ user, open, onClose }: Props) {
  const updateProfile = useUpdateUserProfileMutation();

  const {
    control,
    handleSubmit,
    reset,
    formState: { isValid },
  } = useForm<EditUserProfileValues>({
    mode: "onTouched",
    resolver: zodResolver(editUserProfileSchema),
    defaultValues: { email: user.email, fullName: user.fullName ?? "" },
  });

  // Re-seed the form whenever a different row's dialog opens.
  useEffect(() => {
    if (open) reset({ email: user.email, fullName: user.fullName ?? "" });
  }, [open, user, reset]);

  const onSubmit = (values: EditUserProfileValues) => {
    updateProfile.mutate(
      {
        userId: user.id,
        email: values.email,
        fullName: values.fullName?.trim() || null,
      },
      {
        onSuccess: () => {
          toast.success(`${values.email} was updated.`);
          onClose();
        },
        onError: (error) =>
          toast.error(`Failed to update account: ${errorMessage(error)}`),
      }
    );
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit account</DialogTitle>
      <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
        <DialogContent
          sx={{ display: "flex", flexDirection: "column", gap: 2.5, pt: 1 }}
        >
          <TextInput
            label="Email"
            control={control}
            name="email"
            type="email"
            required
            fullWidth
            autoComplete="email"
          />
          <TextInput
            label="Full name"
            control={control}
            name="fullName"
            fullWidth
            autoComplete="name"
          />
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={onClose} disabled={updateProfile.isPending}>
            Cancel
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={updateProfile.isPending || !isValid}
            startIcon={
              updateProfile.isPending ? <CircularProgress size={18} /> : undefined
            }
          >
            {updateProfile.isPending ? "Saving..." : "Save"}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
