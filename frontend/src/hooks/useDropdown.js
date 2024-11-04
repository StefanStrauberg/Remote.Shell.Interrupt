import { useRef, useState } from 'react';

export const useDropdown = (isActiveState = false) => {
    const [isActive, setIsActive] = useState(isActiveState);
    const ref = useRef(null);
    const handleToggle = () => {
        if (isActive) {
            setIsActive(false);
        } else {
            setIsActive(true);
        }
    };

    return {
        ref,
        handleToggle,
        isActive,
    };
};
