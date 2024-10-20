import Text from '../../Text/Text';
import classes from './Button.module.css';

export default function Button({ classNames, children, onClick }) {
    return (
        <button
            onClick={onClick ?? null}
            className={
                classNames ? `${classes.btn} ${classNames}` : classes.btn
            }
        >
            {' '}
            <Text>{children}</Text>{' '}
        </button>
    );
}
