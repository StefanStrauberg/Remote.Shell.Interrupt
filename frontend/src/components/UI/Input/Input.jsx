import classes from './Input.module.css';

export default function Input({ placeholder, onChange, defaultValue }) {
    return (
        <input
            defaultValue={defaultValue ?? ''}
            className={classes.input}
            type="text"
            placeholder={placeholder || ''}
            onChange={onChange}
        />
    );
}
