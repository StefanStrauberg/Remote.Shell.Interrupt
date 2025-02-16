import Text from '../../Text/Text';
import classes from './Button.module.css';

export default function Button({ isCustom, children, onClick, classNames }) {
    return (
        <button
            onClick={onClick ?? null}
            className={
                classNames
                    ? `${classes.btn} ${classNames}`
                    : classes.btn + ' ' + (isCustom ? ' !bg-black ' : '')
            }
        >
            {' '}
            <Text>{children}</Text>{' '}
        </button>
    );
}
