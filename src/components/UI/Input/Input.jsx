import classes from './Input.module.css';

export default function Input({ placeholder }) {
    return (
        <input
            className={classes.input}
            type="text"
            placeholder={placeholder || ''}
        />
    );
}
