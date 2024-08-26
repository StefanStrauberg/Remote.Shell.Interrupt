import classes from './Select.module.css';

export default function Select() {
    return (
        <select className={classes.select}>
            <option value="123">1234</option>
            <option value="123">1235</option>
            <option value="123">1236</option>
            <option value="123">123</option>
        </select>
    );
}
