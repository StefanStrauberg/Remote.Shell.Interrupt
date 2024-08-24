import Wrapper from '../../Wrapper/Wrapper';
import Button from '../Button/Button';
import Input from '../Input/Input';
import classes from './Form.module.css';

export default function Form() {
    return (
        <Wrapper>
            <div className={classes.wrapper}>
                {' '}
                <label>
                    <div className={classes.labelText}>Name</div>
                    <Input placeholder={'Enter a new info'} />
                </label>
                <label>
                    <div className={classes.labelText}>Type of request</div>
                    <Input placeholder={'Enter a new info'} />
                </label>
                <label>
                    <div className={classes.labelText}>Target field name</div>
                    <Input placeholder={'Enter a new info'} />
                </label>
                <label>
                    <div className={classes.labelText}>OID</div>
                    <Input placeholder={'Enter a new info'} />
                </label>
            </div>
            <div className={classes.submitWrapper}>
                <Button classNames={classes.saveBtn}>Сохранить</Button>
            </div>
        </Wrapper>
    );
}
