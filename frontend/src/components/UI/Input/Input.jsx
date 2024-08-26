import classes from './Input.module.css';

export default function Input({ placeholder, onChange }) {
    return (
        <input
            className={classes.input}
            type="text"
            placeholder={placeholder || ''}
            onChange={onChange}
        />
    );
}
