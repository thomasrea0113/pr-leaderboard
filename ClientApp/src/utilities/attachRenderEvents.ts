import { attachHashEvents } from './modal-hash';
import { attachToggleEvents } from './toggle';

export const attachRenderEvents = () => {
    attachHashEvents();
    attachToggleEvents();
};
