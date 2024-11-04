import classes from './Select.module.css';

export default function Select({ options, onChange }) {
    return (
        <select className={classes.select} onChange={onChange}>
            {options.map((option) => (
                <option key={option.value}>{option.option}</option>
            ))}
        </select>
    );
}
