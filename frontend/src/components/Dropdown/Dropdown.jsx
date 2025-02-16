import { useDropdown } from '../../hooks/useDropdown';
import Button from '../UI/Button/Button';
import classes from './Dropdown.module.css';
const Dropdown = ({ children, activeText, hideText, classNames }) => {
    const dropdown = useDropdown();
    return (
        <>
            <Button onClick={dropdown.handleToggle} classNames={classes.btn}>
                {!dropdown.isActive ? activeText : hideText}
            </Button>
            <div
                ref={dropdown.ref}
                className={!dropdown.isActive ? classes.none : ''}
            >
                {children}
            </div>
        </>
    );
};

export default Dropdown;
