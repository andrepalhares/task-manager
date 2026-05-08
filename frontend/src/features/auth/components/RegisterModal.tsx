import { Modal } from "../../../shared/components/Modal";
import { RegisterForm } from "./RegisterForm";

type RegisterModalProps = { open: boolean; onClose: () => void };

export function RegisterModal({ open, onClose }: RegisterModalProps) {
  return (
    <Modal open={open} onClose={onClose} title="Create your account">
      <RegisterForm onSuccess={onClose} />
    </Modal>
  );
}
