import { Modal } from "../../../shared/components/Modal";
import { LoginForm } from "./LoginForm";

type LoginModalProps = { open: boolean; onClose: () => void };

export function LoginModal({ open, onClose }: LoginModalProps) {
  return (
    <Modal open={open} onClose={onClose} title="Log in to your account">
      <LoginForm onSuccess={onClose} />
    </Modal>
  );
}
